using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WorkingWithFiles.Application.Interfaces;
using WorkingWithFiles.Domain.Common;
using WorkingWithFiles.Domain.Entities;

namespace WorkingWithFiles.Infrastructure.Repositories;

public class BulkCopySalesOrderInserter : IBulkInsert
{
    private readonly IDbContextFactory<FilesDbContext> _contextFactory;

    public BulkCopySalesOrderInserter(IDbContextFactory<FilesDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<int> InsertAsync(IReadOnlyCollection<SalesOrder> batches, CancellationToken cancellationToken = default)
    {
        if (batches.Count == 0) return 0;

        using var table = CreateTable();
        foreach (var order in batches)
        {
            table.Rows.Add(
                order.RevisionNumber,
                order.OrderDate,
                order.DueDate,
                order.Status,
                order.OnlineOrderFlag,
                order.SalesOrderNumber,
                order.ShipMethod,
                order.SubTotal,
                order.TaxAmt,
                order.Freight,
                order.TotalDue,
                order.RowGuid,
                order.ModifiedDate,
                (object?)order.ShipDate ?? DBNull.Value,
                (object?)order.PurchaseOrderNumber ?? DBNull.Value,
                (object?)order.AccountNumber ?? DBNull.Value,
                (object?)order.CreditCardApprovalCode ?? DBNull.Value,
                (object?)order.Comment ?? DBNull.Value
            );
        }

        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        await using var connection = new SqlConnection(context.Database.GetConnectionString());
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        using var bulkCopy = new SqlBulkCopy(connection);
        bulkCopy.DestinationTableName = "SalesOrders";
        bulkCopy.BatchSize = Constants.BulkCopyBatchSize;
        bulkCopy.BulkCopyTimeout = 0;

        foreach (DataColumn col in table.Columns)
            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);

        await bulkCopy.WriteToServerAsync(table, cancellationToken).ConfigureAwait(false);

        return batches.Count;
    }

    private DataTable CreateTable()
    {
        var table = new DataTable();

        table.Columns.Add("RevisionNumber", typeof(byte));
        table.Columns.Add("OrderDate", typeof(DateTime));
        table.Columns.Add("DueDate", typeof(DateTime));
        table.Columns.Add("Status", typeof(byte));
        table.Columns.Add("OnlineOrderFlag", typeof(bool));
        table.Columns.Add("SalesOrderNumber", typeof(string));
        table.Columns.Add("ShipMethod", typeof(string));
        table.Columns.Add("SubTotal", typeof(decimal));
        table.Columns.Add("TaxAmt", typeof(decimal));
        table.Columns.Add("Freight", typeof(decimal));
        table.Columns.Add("TotalDue", typeof(decimal));
        table.Columns.Add("RowGuid", typeof(Guid));
        table.Columns.Add("ModifiedDate", typeof(DateTime));
        table.Columns.Add("ShipDate", typeof(DateTime));
        table.Columns.Add("PurchaseOrderNumber", typeof(string));
        table.Columns.Add("AccountNumber", typeof(string));
        table.Columns.Add("CreditCardApprovalCode", typeof(string));
        table.Columns.Add("Comment", typeof(string));

        return table;
    }
}