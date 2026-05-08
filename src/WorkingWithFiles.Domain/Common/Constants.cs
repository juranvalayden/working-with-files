namespace WorkingWithFiles.Domain.Common;

public static class Constants
{
    public const int BatchSize = 1750;
    public const int BulkCopyBatchSize = 10000;
    public const int BufferSize = 64 * 1024;
    public const long MinRecords = 1_000_000;
    public const long MaxRecords = 4_000_000;
    public const string FileName = "sample-sales.csv";
    private const string _folderName = @"solution items\sample files\";

    public static readonly string Directory = Path.Combine(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\..")), _folderName);
    public const string Header = "Id,RevisionNumber,OrderDate,DueDate,Status,OnlineOrderFlag,SalesOrderNumber,ShipMethod,SubTotal,TaxAmt,Freight,TotalDue,RowGuid,ModifiedDate,ShipDate,PurchaseOrderNumber,AccountNumber,CreditCardApprovalCode,Comment";
    public const string HardCodedPath = @"C:\Code\WorkingWithFiles\solution items\sample files\sample-sales.csv";

    public const string InsertRawSql = @"INSERT INTO SalesOrders
(
    RevisionNumber, OrderDate, DueDate, Status, OnlineOrderFlag,
    SalesOrderNumber, ShipMethod, SubTotal, TaxAmt, Freight, TotalDue,
    RowGuid, ModifiedDate, ShipDate, PurchaseOrderNumber, AccountNumber,
    CreditCardApprovalCode, Comment
)
VALUES
(
    @RevisionNumber, @OrderDate, @DueDate, @Status, @OnlineOrderFlag,
    @SalesOrderNumber, @ShipMethod, @SubTotal, @TaxAmt, @Freight, @TotalDue,
    @RowGuid, @ModifiedDate, @ShipDate, @PurchaseOrderNumber, @AccountNumber,
    @CreditCardApprovalCode, @Comment
);
";
}
