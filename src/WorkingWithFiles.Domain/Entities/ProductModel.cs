namespace WorkingWithFiles.Domain.Entities;

public class ProductModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid RowGuid { get; set; }
    public DateTime ModifiedDate { get; set; }

    public string? CatalogDescription { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}