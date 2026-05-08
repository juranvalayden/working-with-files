using WorkingWithFiles.Domain.Entities;

namespace WorkingWithFiles.Application.Interfaces;

public interface ISalesOrderRepository
{
    Task<bool> InsertSalesOrderAsync(SalesOrder salesOrder, CancellationToken cancellationToken);
    Task<int> InsertEfBulkSalesOrderAsync(IReadOnlyCollection<SalesOrder> batches, CancellationToken cancellationToken);
}