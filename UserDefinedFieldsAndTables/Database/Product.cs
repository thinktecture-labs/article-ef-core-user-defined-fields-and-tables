namespace UserDefinedFieldsAndTables.Database;

public class Product
{
   public Guid Id { get; }
   public string Name { get; set; }

   public Product(Guid id, string name)
   {
      Id = id;
      Name = name;
   }
}
