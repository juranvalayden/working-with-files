using WorkingWithFiles.Application;
using WorkingWithFiles.Application.Interfaces;
using WorkingWithFiles.Infrastructure;

var builder = WebApplication.CreateBuilder();

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var sampleFileService = scope.ServiceProvider.GetRequiredService<ISampleFileService>();
    // var numberOfRecords = sampleFileService.GetRandomLong();
    // await sampleFileService.CreateSampleCsvFileAsync(numberOfRecords);

    var hasProcessedLines = await sampleFileService.ProcessLinesAsync();

    Console.WriteLine($"Has processed all lines `{hasProcessedLines}`.");
}

await app.RunAsync();