using System.Diagnostics;
using WorkingWithFiles.Application.Interfaces;
using WorkingWithFiles.Domain.Common;

namespace WorkingWithFiles.Infrastructure.Repositories;

public class SampleFileService : ISampleFileService
{
    private readonly ISalesOrderFactory _salesOrderFactory;

    public SampleFileService(ISalesOrderFactory salesOrderFactory)
    {
        _salesOrderFactory = salesOrderFactory ?? throw new ArgumentNullException(nameof(salesOrderFactory));
    }

    public long GetRandomLong() => Random.Shared.NextInt64(Constants.MinRecords, Constants.MaxRecords + 1);

    /// <summary>
    /// Create a CSV file with generated records. The method is chunked to bound memory usage,
    /// generates records in parallel within each chunk, and writes each chunk as a single payload.
    /// </summary>
    public async Task CreateSampleCsvFileAsync(long numberOfRecords, bool shouldCreateNewFile = true, CancellationToken cancellationToken = default)
    {
        if (numberOfRecords <= 0) numberOfRecords = 10;

        try
        {
            var filePath = await DirectoryFileCheckerAsync(shouldCreateNewFile, cancellationToken).ConfigureAwait(false);

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var token = cts.Token;

            var stopwatch = Stopwatch.StartNew();
            Console.WriteLine($"Start... Writing {numberOfRecords:N0} records at {DateTime.Now:HH:mm:ss}");

            // Tunable: how many records to generate in memory at once
            var chunkSize = Math.Max(1, Constants.BatchSize * 10); // e.g., 10 batches per chunk
            long produced = 0;

            // Start progress reporter (prints every 5 seconds)
            var progressTask = StartProgressReporterAsync(numberOfRecords, () => Interlocked.Read(ref produced), stopwatch, token);

            // Open file once and append chunk-by-chunk
            await using var fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read, 65536, useAsync: true);
            await using var sw = new StreamWriter(fs);

            for (long offset = 0; offset < numberOfRecords; offset += chunkSize)
            {
                token.ThrowIfCancellationRequested();

                var currentChunk = (int)Math.Min(chunkSize, numberOfRecords - offset);
                var lines = new string[currentChunk];

                // Parallel generation into indexed array
                var po = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
                Parallel.For(0, currentChunk, po, i =>
                {
                    // Factory must be thread-safe (ThreadLocal Faker + Interlocked id)
                    var dto = _salesOrderFactory.CreateFakeDto();
                    lines[i] = dto.ToString();
                    Interlocked.Increment(ref produced);
                });

                // Write chunk in-order as a single payload to reduce syscalls
                var payload = string.Join(Environment.NewLine, lines) + Environment.NewLine;
                await sw.WriteAsync(payload).ConfigureAwait(false);
                await sw.FlushAsync(token).ConfigureAwait(false);
            }

            // Stop progress reporter
            await cts.CancelAsync();
            try { await progressTask.ConfigureAwait(false); } catch { /* ignore cancellation */ }

            stopwatch.Stop();
            Console.WriteLine($"Finish... Completed at {DateTime.Now:HH:mm:ss} | Total time: {stopwatch.Elapsed}");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("[CANCELLED] Operation was cancelled.");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] {ex.Message}");
            throw;
        }
    }

    public async Task<string[]> ReadSampleCsvAsync()
    {
        var filePathAndFileName = Path.Combine(Constants.Directory, Constants.FileName);

        if (!File.Exists(filePathAndFileName))
        {
            return Array.Empty<string>();
        }

        return await File.ReadAllLinesAsync(filePathAndFileName).ConfigureAwait(false);
    }

    private async Task<string> DirectoryFileCheckerAsync(bool shouldCreateNewFile, CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(Constants.Directory))
        {
            Directory.CreateDirectory(Constants.Directory);
        }

        var filePathAndFileName = Path.Combine(Constants.Directory, Constants.FileName);

        if (shouldCreateNewFile && File.Exists(filePathAndFileName))
        {
            File.Delete(filePathAndFileName);
        }

        if (!File.Exists(filePathAndFileName))
        {
            // create file with header
            await File.AppendAllTextAsync(filePathAndFileName, Constants.Header + Environment.NewLine, cancellationToken).ConfigureAwait(false);
        }

        return filePathAndFileName;
    }

    private static Task StartProgressReporterAsync(long total, Func<long> readProduced, Stopwatch stopwatch, CancellationToken token)
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
                    Console.WriteLine($@"Progress: {percent,3}%  Records: {produced:N0}/{total:N0}  Elapsed: {stopwatch.Elapsed:hh\:mm\:ss}");
                }
            }
            catch (OperationCanceledException) { /* expected on shutdown */ }
        }, token);
    }
}
