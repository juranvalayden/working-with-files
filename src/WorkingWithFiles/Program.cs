using WorkingWithFiles.Application;
using WorkingWithFiles.Application.Interfaces;
using WorkingWithFiles.Infrastructure;

var builder = WebApplication.CreateBuilder();

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var salesOrderFactory = scope.ServiceProvider.GetRequiredService<ISalesOrderFactory>();
    var fakeSalesOrderDto = salesOrderFactory.CreateFakeDto();
}


await app.RunAsync();