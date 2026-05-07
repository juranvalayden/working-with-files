using Microsoft.Extensions.DependencyInjection;
using WorkingWithFiles.Application.Interfaces;
using WorkingWithFiles.Infrastructure.Repositories;

namespace WorkingWithFiles.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ISampleFileService, SampleFileService>();
        services.AddSingleton<ISalesOrderFactory, SalesOrderFactory>();


    }
}