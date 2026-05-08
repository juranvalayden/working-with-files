namespace WorkingWithFiles.Application.Interfaces;

public interface ISampleFileService
{
    long GetRandomLong();
    Task CreateSampleCsvFileAsync(long numberOfRecords, bool shouldCreateNewFile = true, CancellationToken cancellationToken = default);
    Task<bool> ProcessLinesAsync(CancellationToken cancellationToken = default);
}