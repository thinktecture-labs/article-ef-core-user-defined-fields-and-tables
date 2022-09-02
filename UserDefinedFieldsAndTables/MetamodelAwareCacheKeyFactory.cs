using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace UserDefinedFieldsAndTables;

public class MetamodelAwareCacheKeyFactory : IModelCacheKeyFactory
{
   public object Create(DbContext context, bool designTime)
   {
      return context is IMetamodelAccessor metamodelAccessor
                ? new MetamodelCacheKey(context, designTime, metamodelAccessor.Metamodel.Version)
                : new ModelCacheKey(context, designTime);
   }
}
