namespace UserDefinedFieldsAndTables;

public class AdditionalField
{
   public string EntityName { get; set; }
   public string PropertyName { get; set; }
   public Type PropertyType { get; set; }
   public bool IsRequired { get; set; }
   public int? MaxLength { get; set; }
}
