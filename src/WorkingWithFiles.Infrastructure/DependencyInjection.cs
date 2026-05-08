using Microsoft.Extensions.DependencyInjection;
using WorkingWithFiles.Application.Interfaces;
using WorkingWithFiles.Infrastructure.Factories;
using WorkingWithFiles.Infrastructure.Mappers;
using WorkingWithFiles.Infrastructure.Services;

namespace WorkingWithFiles.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ISampleFileService, SampleFileService>();
        
        services.AddSingleton<IMapper, LineMapper>();
        services.AddSingleton<ISalesOrderFactory, SalesOrderFactory>();
    }
}