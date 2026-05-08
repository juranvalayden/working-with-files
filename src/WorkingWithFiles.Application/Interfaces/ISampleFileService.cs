using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace WorkingWithFiles.Application.Interfaces
{
    /// <summary>
    /// Service for creating and reading sample CSV files.
    /// </summary>
    public interface ISampleFileService
    {
        /// <summary>
        /// Returns a random number of records within configured bounds.
        /// </summary>
        long GetRandomLong();

        /// <summary>
        /// Create a CSV file with generated records.
        /// </summary>
        /// <param name="numberOfRecords">Number of records to generate.</param>
        /// <param name="shouldCreateNewFile">If true, replace any existing file.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task CreateSampleCsvFileAsync(long numberOfRecords, bool shouldCreateNewFile = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Stream file lines asynchronously. Caller may skip the header line.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        IAsyncEnumerable<string> ReadSampleCsvLinesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default);

        /// <summary>
        /// Read and process the CSV file (example returns processed line count).
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<int> ReadSampleCsvAsync(CancellationToken cancellationToken = default);
    }
}