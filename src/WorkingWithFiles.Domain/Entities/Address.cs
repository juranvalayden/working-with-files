namespace WorkingWithFiles.Domain.Entities;

public class Address
{
    public int Id { get; set; }
    public Guid RowGuid { get; set; }
    public DateTime ModifiedDate { get; set; }

    public string AddressLine1 { get; set; } = null!;
    public string City { get; set; } = null!;
    public string StateProvince { get; set; } = null!;
    public string CountryRegion { get; set; } = null!;
    public string PostalCode { get; set; } = null!;

    public string? AddressLine2 { get; set; }

    public ICollection<SalesOrder> ShipToSalesOrders { get; set; } = new List<SalesOrder>();
    public ICollection<SalesOrder> BillToSalesOrders { get; set; } = new List<SalesOrder>();
}
