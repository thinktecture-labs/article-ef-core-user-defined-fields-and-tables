using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace UserDefinedFieldsAndTables;

public sealed class MetamodelCacheKey : ModelCacheKey
{
   private readonly int _metamodelVersion;

   public MetamodelCacheKey(DbContext context, bool designTime, int metamodelVersion)
      : base(context, designTime)
   {
      _metamodelVersion = metamodelVersion;
   }

   protected override bool Equals(ModelCacheKey other)
   {
      return other is MetamodelCacheKey otherCacheKey
             && base.Equals(otherCacheKey)
             && otherCacheKey._metamodelVersion == _metamodelVersion;
   }

   public override int GetHashCode()
   {
      return HashCode.Combine(base.GetHashCode(), _metamodelVersion);
   }
}
