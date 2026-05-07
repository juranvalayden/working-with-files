namespace WorkingWithFiles.Application.Dtos;

public record SalesOrderDto
{
    public int Id { get; set; }
    public byte RevisionNumber { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime DueDate { get; set; }
    public byte Status { get; set; }
    public bool OnlineOrderFlag { get; set; }
    public string SalesOrderNumber { get; set; } = null!;
    public string ShipMethod { get; set; } = null!;
    public decimal SubTotal { get; set; }
    public decimal TaxAmt { get; set; }
    public decimal Freight { get; set; }
    public decimal TotalDue { get; set; }
    public Guid RowGuid { get; set; }
    public DateTime ModifiedDate { get; set; }

    public DateTime? ShipDate { get; set; }
    public string? PurchaseOrderNumber { get; set; }
    public string? AccountNumber { get; set; }
    public string? CreditCardApprovalCode { get; set; }
    public string? Comment { get; set; }
}


