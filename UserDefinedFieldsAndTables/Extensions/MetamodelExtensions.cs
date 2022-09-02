using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// ReSharper disable once CheckNamespace
namespace UserDefinedFieldsAndTables;

public static class MetamodelExtensions
{
   public static void ApplyChanges(
      this Metamodel metamodel,
      ModelBuilder modelBuilder)
   {
      foreach (var entity in metamodel.Entities)
      {
         modelBuilder.AddEntity(entity);
      }

      foreach (var fieldGroup in metamodel.Fields.GroupBy(f => f.EntityName))
      {
         modelBuilder.Entity(fieldGroup.Key,
                             builder =>
                             {
                                foreach (var field in fieldGroup)
                                {
                                   builder.AddField(field);
                                }
                             });
      }
   }

   private static void AddEntity(this ModelBuilder modelBuilder, AdditionalEntity entity)
   {
      modelBuilder.Entity(entity.EntityName,
                          builder =>
                          {
                             builder.ToTable(entity.TableName, entity.TableSchema);

                             foreach (var field in entity.Fields)
                             {
                                builder.AddField(field);
                             }

                             if (entity.Key.Count == 0)
                             {
                                builder.HasNoKey();
                             }
                             else
                             {
                                builder.HasKey(entity.Key.Select(f => f.PropertyName).ToArray());
                             }
                          });
   }

   private static void AddField(this EntityTypeBuilder builder, AdditionalField field)
   {
      var propertyBuilder = builder.Property(field.PropertyType, field.PropertyName)
                                   .IsRequired(field.IsRequired);

      if (field.MaxLength.HasValue)
         propertyBuilder.HasMaxLength(field.MaxLength.Value);
   }
}
