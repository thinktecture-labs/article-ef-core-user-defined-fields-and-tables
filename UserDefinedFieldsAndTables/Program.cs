using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UserDefinedFieldsAndTables.Database;

var connString = "server=localhost;database=UserDefinedFieldsAndTables;integrated security=true";

var serviceProvider = new ServiceCollection()
                      .AddDbContext<DemoDbContext>(builder => builder.UseSqlServer(connString))
                      .BuildServiceProvider();

await ReCreateDatabaseAndFetchProductsAsync(serviceProvider);

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
