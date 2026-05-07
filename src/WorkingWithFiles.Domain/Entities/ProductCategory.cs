namespace WorkingWithFiles.Domain.Entities;

public class ProductCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid RowGuid { get; set; }
    public DateTime ModifiedDate { get; set; }

    public int? ParentProductCategoryId { get; set; }
    public ProductCategory? ParentProductCategory { get; set; }

    public ICollection<ProductCategory> ChildProductCategories { get; set; } = new List<ProductCategory>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}