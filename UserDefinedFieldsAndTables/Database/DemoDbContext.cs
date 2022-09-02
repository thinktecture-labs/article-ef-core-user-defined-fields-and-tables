using Microsoft.EntityFrameworkCore;

namespace UserDefinedFieldsAndTables.Database;

public class DemoDbContext : DbContext, IMetamodelAccessor
{
   public Metamodel Metamodel { get; }

   public DbSet<Product> Products { get; set; }

   public DemoDbContext(
      DbContextOptions<DemoDbContext> options,
      Metamodel metamodel)
      : base(options)
   {
      Metamodel = metamodel;
   }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      modelBuilder.Entity<Product>(builder =>
                                   {
                                      builder.HasKey(p => p.Id);
                                      builder.Property(p => p.Name).HasMaxLength(100);
                                   });

      Metamodel.ApplyChanges(modelBuilder);
   }
}
