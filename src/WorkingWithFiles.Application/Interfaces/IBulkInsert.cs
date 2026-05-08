using WorkingWithFiles.Domain.Entities;

namespace WorkingWithFiles.Application.Interfaces;

public interface IBulkInsert
{
    Task<int> InsertAsync(IReadOnlyCollection<SalesOrder> batches, CancellationToken cancellationToken = default);
}