using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using UserDefinedFieldsAndTables;
using UserDefinedFieldsAndTables.Database;

var connString = "server=localhost;database=UserDefinedFieldsAndTables;integrated security=true";

var serviceProvider = new ServiceCollection()
                      .AddSingleton<Metamodel>()
                      .AddDbContext<DemoDbContext>(builder => builder.UseSqlServer(connString)
                                                                     .ReplaceService<IModelCacheKeyFactory, MetamodelAwareCacheKeyFactory>())
                      .BuildServiceProvider();

await ReCreateDatabaseAndFetchProductsAsync(serviceProvider);
await ChangeModelAsync(serviceProvider);
await AccessDescriptionAsync(serviceProvider);
await AccessProductTypeAsync(serviceProvider);

static async Task AccessProductTypeAsync(ServiceProvider provider)
{
   await using var scope = provider.CreateAsyncScope();

   var dbContext = scope.ServiceProvider.GetRequiredService<DemoDbContext>();

   var names = await dbContext.Set<Dictionary<string, object>>("ProductType")
                              .Where(p => EF.Property<string?>(p, "Name") != String.Empty)
                              .OrderBy(p => EF.Property<string?>(p, "Name"))
                              .Select(p => EF.Property<string?>(p, "Name"))
                              .ToListAsync();

   // output: ["ProductType"]
   Console.WriteLine(JsonSerializer.Serialize(names));

   var productTypes = await dbContext.Set<Dictionary<string, object>>("ProductType")
                                     .ToListAsync();

   // [{"Id":"5b3f23f9-9d97-42a2-99f2-1d19710e6690","Name":"ProductType"}]
   Console.WriteLine(JsonSerializer.Serialize(productTypes));
}

static async Task AccessDescriptionAsync(ServiceProvider provider)
{
   await using var scope = provider.CreateAsyncScope();

   var dbContext = scope.ServiceProvider.GetRequiredService<DemoDbContext>();

   var descriptions = await dbContext.Products
                                     .Where(p => EF.Property<string?>(p, "Description") != null)
                                     .OrderBy(p => EF.Property<string?>(p, "Description"))
                                     .Select(p => EF.Property<string?>(p, "Description"))
                                     .ToListAsync();

   // output: ["Product description"]
   Console.WriteLine(JsonSerializer.Serialize(descriptions));

   var product = await dbContext.Products.SingleAsync();

   // output: {"Id":"3cb4a79e-17df-4f3f-8a5f-62561153e789","Name":"Product"}
   Console.WriteLine(JsonSerializer.Serialize(product));

   var description = dbContext.Entry(product).Property<string>("Description").CurrentValue;

   // output: Product description
   Console.WriteLine(description);
}

static async Task ChangeModelAsync(ServiceProvider provider)
{
   await using var scope = provider.CreateAsyncScope();

   var dbContext = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
   var metamodel = scope.ServiceProvider.GetRequiredService<Metamodel>();

   metamodel.Version++;

   // Add a new field to existing entity
   metamodel.Fields.Add(new AdditionalField
                        {
                           EntityName = "UserDefinedFieldsAndTables.Database.Product",
                           PropertyName = "Description",
                           PropertyType = typeof(string),
                           MaxLength = 200
                        });

   // Add completely new entity
   var productTypeKey = new AdditionalField
                        {
                           PropertyName = "Id",
                           PropertyType = typeof(Guid),
                           IsRequired = true
                        };

   metamodel.Entities.Add(new AdditionalEntity
                          {
                             EntityName = "ProductType",
                             TableName = "ProductTypes",
                             Key = { productTypeKey },
                             Fields =
                             {
                                productTypeKey,
                                new AdditionalField
                                {
                                   PropertyName = "Name",
                                   PropertyType = typeof(string),
                                   MaxLength = 100
                                }
                             }
                          });

   dbContext.Database.ExecuteSqlRaw(@"
ALTER TABLE Products ADD Description NVARCHAR(200);

CREATE TABLE ProductTypes
(
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL
);
");

   dbContext.Database.ExecuteSqlRaw(@"
UPDATE Products
SET Description = 'Product description';

INSERT INTO ProductTypes (Id, Name)
VALUES ('5B3F23F9-9D97-42A2-99F2-1D19710E6690', 'ProductType');
");
}

static async Task ReCreateDatabaseAndFetchProductsAsync(ServiceProvider provider)
{
   await using var scope = provider.CreateAsyncScope();

   var dbContext = scope.ServiceProvider.GetRequiredService<DemoDbContext>();

   await dbContext.Database.EnsureDeletedAsync();
   await dbContext.Database.EnsureCreatedAsync();

   if (!dbContext.Products.Any())
   {
      var id = new Guid("3CB4A79E-17DF-4F3F-8A5F-62561153E789");
      dbContext.Products.Add(new Product(id, "Product"));

      await dbContext.SaveChangesAsync();
   }

   var products = await dbContext.Products.ToListAsync();
   Console.WriteLine(JsonSerializer.Serialize(products));
}
