namespace UserDefinedFieldsAndTables;

public class Metamodel
{
   public int Version { get; set; }

   public List<AdditionalField> Fields { get; } = new();
   public List<AdditionalEntity> Entities { get; } = new();
}
