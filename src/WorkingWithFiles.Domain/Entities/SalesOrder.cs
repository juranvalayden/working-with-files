namespace WorkingWithFiles.Domain.Entities;

public class SalesOrder
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

    public SalesOrder()
    {
        
    }
}

//public class SalesOrder
//{
//    public int Id { get; set; }
//    public byte RevisionNumber { get; set; }
//    public DateTime OrderDate { get; set; }
//    public DateTime DueDate { get; set; }
//    public byte Status { get; set; }
//    public bool OnlineOrderFlag { get; set; }
//    public string SalesOrderNumber { get; set; } = null!;
//    public string ShipMethod { get; set; } = null!;
//    public decimal SubTotal { get; set; }
//    public decimal TaxAmt { get; set; }
//    public decimal Freight { get; set; }
//    public decimal TotalDue { get; set; }
//    public Guid RowGuid { get; set; }
//    public DateTime ModifiedDate { get; set; }

//    public DateTime? ShipDate { get; set; }
//    public string? PurchaseOrderNumber { get; set; }
//    public string? AccountNumber { get; set; }
//    public string? CreditCardApprovalCode { get; set; }
//    public string? Comment { get; set; }

//    public int? ShipToAddressId { get; set; }
//    public Address? ShipToAddress { get; set; }

//    public int? BillToAddressId { get; set; }
//    public Address? BillToAddress { get; set; }

//    public int CustomerId { get; set; }
//    public Customer Customer { get; set; } = null!;

//    public ICollection<SalesOrderDetail> SalesOrderDetails { get; set; } = new List<SalesOrderDetail>();
//    public decimal TaxAmount { get; set; }
//}
