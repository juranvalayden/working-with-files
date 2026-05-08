using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using WorkingWithFiles.Application.Interfaces;
using WorkingWithFiles.Domain.Common;
using WorkingWithFiles.Domain.Entities;

namespace WorkingWithFiles.Infrastructure.Repositories;

public class SalesOrderRepository : ISalesOrderRepository
{
    private readonly IDbContextFactory<FilesDbContext> _contextFactory;
    private readonly ILogger<SalesOrderRepository> _logger;
    private readonly DataTable _table;

    public SalesOrderRepository(IDbContextFactory<FilesDbContext> contextFactory, ILogger<SalesOrderRepository> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
        _table = CreateTable();
    }

    public async Task<bool> InsertSalesOrderAsync(SalesOrder salesOrder, CancellationToken cancellationToken)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var parameters = GetInsertParameters(salesOrder);
            var rowsAffected = await context.Database.ExecuteSqlRawAsync(Constants.InsertRawSql, parameters, cancellationToken);

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Single insert failed for SalesOrder {SalesOrderNumber}", salesOrder.SalesOrderNumber);
            throw;
        }
    }

    public async Task<int> InsertEfBulkSalesOrderAsync(IReadOnlyCollection<SalesOrder> batches, CancellationToken cancellationToken)
    {
        if (batches.Count == 0) return 0;

        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        try
        {
            context.ChangeTracker.AutoDetectChangesEnabled = false;

            await context.SalesOrders.AddRangeAsync(batches, cancellationToken).ConfigureAwait(false);
            var savedChanges = await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return savedChanges;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bulk insert failed for {Count} SalesOrders", batches.Count);
            return 0;
        }
        finally
        {
            context.ChangeTracker.AutoDetectChangesEnabled = true;
        }
    }

    private static SqlParameter[] GetInsertParameters(SalesOrder entity)
    {
        return
        [
            new SqlParameter("@RevisionNumber", SqlDbType.TinyInt) { Value = entity.RevisionNumber },
            new SqlParameter("@OrderDate", SqlDbType.Date) { Value = entity.OrderDate },
            new SqlParameter("@DueDate", SqlDbType.Date) { Value = entity.DueDate },
            new SqlParameter("@Status", SqlDbType.TinyInt) { Value = entity.Status },
            new SqlParameter("@OnlineOrderFlag", SqlDbType.Bit) { Value = entity.OnlineOrderFlag },
            new SqlParameter("@SalesOrderNumber", SqlDbType.NVarChar, 50) { Value = entity.SalesOrderNumber },
            new SqlParameter("@ShipMethod", SqlDbType.NVarChar, 50) { Value = entity.ShipMethod },
            new SqlParameter("@SubTotal", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = entity.SubTotal },
            new SqlParameter("@TaxAmt", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = entity.TaxAmt },
            new SqlParameter("@Freight", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = entity.Freight },
            new SqlParameter("@TotalDue", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = entity.TotalDue },
            new SqlParameter("@RowGuid", SqlDbType.UniqueIdentifier) { Value = entity.RowGuid },
            new SqlParameter("@ModifiedDate", SqlDbType.Date) { Value = entity.ModifiedDate },
            new SqlParameter("@ShipDate", SqlDbType.Date) { Value = (object?)entity.ShipDate ?? DBNull.Value },
            new SqlParameter("@PurchaseOrderNumber", SqlDbType.NVarChar, 25) { Value = (object?)entity.PurchaseOrderNumber ?? DBNull.Value },
            new SqlParameter("@AccountNumber", SqlDbType.NVarChar, 25) { Value = (object?)entity.AccountNumber ?? DBNull.Value },
            new SqlParameter("@CreditCardApprovalCode", SqlDbType.NVarChar, 25) { Value = (object?)entity.CreditCardApprovalCode ?? DBNull.Value },
            new SqlParameter("@Comment", SqlDbType.NVarChar, -1) { Value = (object?)entity.Comment ?? DBNull.Value }
        ];
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

    public async Task InsertBulkCopy(
        IReadOnlyCollection<SalesOrder> batches,
        CancellationToken cancellationToken = default)
    {
        if (batches.Count == 0) return;

        var table = CreateTable();

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

        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        await using var connection = new SqlConnection(context.Database.GetConnectionString());
        await connection.OpenAsync(cancellationToken);

        using var bulkCopy = new SqlBulkCopy(connection);
        bulkCopy.DestinationTableName = "SalesOrders";
        bulkCopy.BatchSize = 5000;
        bulkCopy.BulkCopyTimeout = 0;

        foreach (DataColumn col in table.Columns) bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);

        await bulkCopy.WriteToServerAsync(table, cancellationToken);

        table.Dispose(); // release memory
    }

}
