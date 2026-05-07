namespace WorkingWithFiles.Domain.Entities;

public class SalesOrderDetail
{
    public int Id { get; set; }
    public short OrderQty { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal UnitPriceDiscount { get; set; }
    public decimal LineTotal { get; set; }
    public Guid RowGuid { get; set; }
    public DateTime ModifiedDate { get; set; }

    public int SalesOrderId { get; set; }
    public SalesOrder SalesOrder { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}
