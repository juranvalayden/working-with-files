using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WorkingWithFiles.Application.Interfaces;
using WorkingWithFiles.Domain.Entities;

namespace WorkingWithFiles.Infrastructure.Repositories;

public class EfSalesOrderInserter : IBulkInsert
{
    private readonly IDbContextFactory<FilesDbContext> _contextFactory;
    private readonly ILogger<EfSalesOrderInserter> _logger;

    public EfSalesOrderInserter(IDbContextFactory<FilesDbContext> contextFactory, ILogger<EfSalesOrderInserter> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task<int> InsertAsync(IReadOnlyCollection<SalesOrder> batches, CancellationToken cancellationToken = default)
    {
        if (batches.Count == 0) return 0;

        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        try
        {
            context.ChangeTracker.AutoDetectChangesEnabled = false;
            await context.SalesOrders.AddRangeAsync(batches, cancellationToken);
            return await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "EF bulk insert failed for {Count} SalesOrders", batches.Count);
            return 0;
        }
        finally
        {
            context.ChangeTracker.AutoDetectChangesEnabled = true;
        }
    }
}