namespace WorkingWithFiles.Domain.Common;

public static class Constants
{
    public const int BatchSize = 2500;
    public const long MinRecords = 1_000_000;
    public const long MaxRecords = 4_000_000;
    public const string FileName = "sample-sales.csv";
    private const string _folderName = @"solution items\sample files\";

    public static readonly string Directory = Path.Combine(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\..")), _folderName);
    public const string Header = "Id,RevisionNumber,OrderDate,DueDate,Status,OnlineOrderFlag,SalesOrderNumber,ShipMethod,SubTotal,TaxAmt,Freight,TotalDue,RowGuid,ModifiedDate,ShipDate,PurchaseOrderNumber,AccountNumber,CreditCardApprovalCode,Comment";
    
}
