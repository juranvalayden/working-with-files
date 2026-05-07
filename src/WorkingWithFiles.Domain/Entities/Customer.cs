namespace WorkingWithFiles.Domain.Entities;

public class Customer
{
    public int Id { get; set; }
    public bool NameStyle { get; set; }
    public Guid RowGuid { get; set; }
    public DateTime ModifiedDate { get; set; }

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string PasswordSalt { get; set; } = null!;

    public string? Title { get; set; }
    public string? MiddleName { get; set; }
    public string? Suffix { get; set; }
    public string? CompanyName { get; set; }
    public string? SalesPerson { get; set; }
    public string? EmailAddress { get; set; }
    public string? Phone { get; set; }

    public ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();
}
