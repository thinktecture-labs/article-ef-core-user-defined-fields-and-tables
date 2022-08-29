using Microsoft.EntityFrameworkCore;

namespace UserDefinedFieldsAndTables.Database;

public class DemoDbContext : DbContext
{
   public DbSet<Product> Products { get; set; }

   public DemoDbContext(
      DbContextOptions<DemoDbContext> options)
      : base(options)
   {
   }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      modelBuilder.Entity<Product>(builder =>
                                   {
                                      builder.HasKey(p => p.Id);
                                      builder.Property(p => p.Name).HasMaxLength(100);
                                   });
   }
}
