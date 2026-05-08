namespace WorkingWithFiles.Application.Dtos;

public record SalesOrderDto
{
    public int Id { get; init; }
    public byte RevisionNumber { get; init; }
    public DateTime OrderDate { get; init; }
    public DateTime DueDate { get; init; }
    public byte Status { get; init; }
    public bool OnlineOrderFlag { get; init; }
    public string SalesOrderNumber { get; init; } = null!;
    public string ShipMethod { get; init; } = null!;
    public decimal SubTotal { get; init; }
    public decimal TaxAmt { get; init; }
    public decimal Freight { get; init; }
    public decimal TotalDue { get; init; }
    public Guid RowGuid { get; init; }
    public DateTime ModifiedDate { get; init; }

    public DateTime? ShipDate { get; init; }
    public string? PurchaseOrderNumber { get; init; }
    public string? AccountNumber { get; init; }
    public string? CreditCardApprovalCode { get; init; }
    public string? Comment { get; init; }

    public override string ToString()
    {
        return string.Join(",",
            Id,
            RevisionNumber,
            OrderDate.ToString("yyyy-MM-dd"),
            DueDate.ToString("yyyy-MM-dd"),
            Status,
            OnlineOrderFlag,
            SalesOrderNumber,
            ShipMethod,
            SubTotal,
            TaxAmt,
            Freight,
            TotalDue,
            RowGuid,
            ModifiedDate.ToString("yyyy-MM-dd"),
            ShipDate?.ToString("yyyy-MM-dd") ?? string.Empty,
            PurchaseOrderNumber ?? string.Empty,
            AccountNumber ?? string.Empty,
            CreditCardApprovalCode ?? string.Empty,
            Comment ?? string.Empty
        );
    }
}