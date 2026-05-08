using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using WorkingWithFiles.Application.Interfaces;
using WorkingWithFiles.Domain.Common;
using WorkingWithFiles.Domain.Entities;

namespace WorkingWithFiles.Infrastructure.Services;

public class SampleFileService(ISalesOrderRepository salesOrderRepository,
    IBulkInsert bulkInsert,
    ISalesOrderFactory salesOrderFactory,
    IMapper mapper) : ISampleFileService
{
    public long GetRandomLong() =>
        Random.Shared.NextInt64(Constants.MinRecords, Constants.MaxRecords + 1);

    public async Task CreateSampleCsvFileAsync(
        long numberOfRecords,
        bool shouldCreateNewFile = true,
        CancellationToken cancellationToken = default)
    {
        if (numberOfRecords <= 0) numberOfRecords = 10;

        var filePath = await DirectoryFileCheckerAsync(shouldCreateNewFile, cancellationToken).ConfigureAwait(false);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var token = cts.Token;

        var stopwatch = Stopwatch.StartNew();
        Console.WriteLine($"Start... Writing {numberOfRecords:N0} records at {DateTime.Now:HH:mm:ss}");

        var chunkSize = Math.Max(1, Constants.BulkCopyBatchSize * 10);
        long produced = 0;

        var progressTask = StartProgressReporterAsync(
            numberOfRecords,
            () => Interlocked.Read(ref produced),
            stopwatch,
            token);

        await using var fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read, 65536, useAsync: true);
        await using var sw = new StreamWriter(fs);

        for (long offset = 0; offset < numberOfRecords; offset += chunkSize)
        {
            token.ThrowIfCancellationRequested();

            var currentChunk = (int)Math.Min(chunkSize, numberOfRecords - offset);
            var lines = new string[currentChunk];

            Parallel.For(0, currentChunk, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, i =>
            {
                var dto = salesOrderFactory.CreateFakeDto();
                lines[i] = dto.ToString();
                Interlocked.Increment(ref produced);
            });

            var payload = string.Join(Environment.NewLine, lines) + Environment.NewLine;
            await sw.WriteAsync(payload).ConfigureAwait(false);
            await sw.FlushAsync(token).ConfigureAwait(false);
        }

        await cts.CancelAsync();
        try { await progressTask.ConfigureAwait(false); } catch { /* ignore cancellation */ }

        stopwatch.Stop();
        Console.WriteLine($"Finish... Completed at {DateTime.Now:HH:mm:ss} | Total time: {stopwatch.Elapsed}");
    }

    public async Task<bool> ProcessLinesAsync(CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(Constants.Directory, Constants.FileName);
        if (!File.Exists(filePath)) return false;

        var skipHeader = true;
        var batches = new List<SalesOrder>(Constants.BatchSize);
        var rowsInserted = 0;

        await foreach (var line in StreamCsvLinesAsync(filePath, cancellationToken))
        {
            if (skipHeader) { skipHeader = false; continue; }
            if (string.IsNullOrWhiteSpace(line)) continue;

            var salesOrder = mapper.MapDtoToEntity(line.Split(','));
            batches.Add(salesOrder);

            if (batches.Count < Constants.BatchSize) continue;

            rowsInserted = await bulkInsert.InsertAsync(batches, cancellationToken).ConfigureAwait(false);

            batches.Clear();
        }

        if (batches.Count > 0) rowsInserted = await salesOrderRepository.InsertEfBulkSalesOrderAsync(batches, cancellationToken);

        return rowsInserted > 0;
    }

    public async Task<bool> ProcessLinesWithReportingAsync(CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(Constants.Directory, Constants.FileName);
        if (!File.Exists(filePath)) return false;

        var skipHeader = true;

        var batches = new List<SalesOrder>(Constants.BulkCopyBatchSize);
        var lineCount = 0;
        long produced = 0;

        var stopwatch = Stopwatch.StartNew();
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var total = File.ReadLines(filePath).Count() - 1;
        var reporterTask = StartProgressReporterAsync(total, () => Interlocked.Read(ref produced), stopwatch, cts.Token);

        await foreach (var line in StreamCsvLinesAsync(filePath, cancellationToken))
        {
            if (skipHeader) { skipHeader = false; continue; }
            if (string.IsNullOrWhiteSpace(line)) continue;

            var salesOrder = mapper.MapDtoToEntity(line.Split(','));
            batches.Add(salesOrder);

            if (batches.Count < Constants.BulkCopyBatchSize) continue;

            var inserted = await bulkInsert.InsertAsync(batches, cancellationToken);
            if (inserted == 0) break;

            Interlocked.Add(ref produced, batches.Count);
            batches.Clear();
            lineCount++;
        }

        if (batches.Count > 0)
        {
            var inserted = await salesOrderRepository.InsertEfBulkSalesOrderAsync(batches, cancellationToken);
            if (inserted != 0)
            {
                Interlocked.Add(ref produced, batches.Count);
                lineCount++;
            }
        }

        await cts.CancelAsync();
        await reporterTask;
        stopwatch.Stop();

        return lineCount > 0;
    }

    private static async IAsyncEnumerable<string> StreamCsvLinesAsync(
        string path,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var fileStreamOptions = CreateFileStreamOptions();
        await using var inputFileStream = new FileStream(path, fileStreamOptions);
        using var reader = new StreamReader(inputFileStream, Encoding.UTF8, true, Constants.BufferSize, false);

        while (!cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false);
            if (line is null) yield break;
            yield return line;
        }
    }

    private static FileStreamOptions CreateFileStreamOptions() => new()
    {
        Mode = FileMode.Open,
        Access = FileAccess.Read,
        Share = FileShare.ReadWrite,
        BufferSize = Constants.BufferSize,
        Options = FileOptions.Asynchronous | FileOptions.SequentialScan
    };

    private static Task StartProgressReporterAsync(
        long total,
        Func<long> readProduced,
        Stopwatch stopwatch,
        CancellationToken token)
    {
        return Task.Run(async () =>
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), token).ConfigureAwait(false);
                    var produced = readProduced();
                    var percent = (int)(produced * 100 / Math.Max(1, total));
                    Console.WriteLine(
                        $"Progress: {percent,3}%  Records: {produced:N0}/{total:N0}  Elapsed: {stopwatch.Elapsed:hh\\:mm\\:ss}");
                }
            }
            catch (OperationCanceledException) { /* expected */ }
        }, token);
    }

    private static async Task<string> DirectoryFileCheckerAsync(
        bool shouldCreateNewFile,
        CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(Constants.Directory))
            Directory.CreateDirectory(Constants.Directory);

        var filePath = Path.Combine(Constants.Directory, Constants.FileName);

        if (shouldCreateNewFile && File.Exists(filePath))
            File.Delete(filePath);

        if (!File.Exists(filePath))
            await File.AppendAllTextAsync(filePath, Constants.Header + Environment.NewLine, cancellationToken).ConfigureAwait(false);

        return filePath;
    }
}
