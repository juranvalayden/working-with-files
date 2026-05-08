using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WorkingWithFiles.Application.Interfaces;
using WorkingWithFiles.Domain.Common;
using WorkingWithFiles.Domain.Entities;

namespace WorkingWithFiles.Infrastructure.Repositories;

public class RawSqlSalesOrderInserter : IBulkInsert
{
    private readonly IDbContextFactory<FilesDbContext> _contextFactory;
    private readonly ILogger<RawSqlSalesOrderInserter> _logger;

    public RawSqlSalesOrderInserter(IDbContextFactory<FilesDbContext> contextFactory, ILogger<RawSqlSalesOrderInserter> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task<int> InsertAsync(IReadOnlyCollection<SalesOrder> batches, CancellationToken cancellationToken = default)
    {
        if (batches.Count == 0) return 0;

        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var total = 0;

        foreach (var order in batches)
        {
            try
            {
                var parameters = GetInsertParameters(order);
                total += await context.Database.ExecuteSqlRawAsync(Constants.InsertRawSql, parameters, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Raw SQL insert failed for SalesOrder {SalesOrderNumber}", order.SalesOrderNumber);
            }
        }

        return total;
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
}