using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WorkingWithFiles.Application.Interfaces;
using WorkingWithFiles.Infrastructure.Factories;
using WorkingWithFiles.Infrastructure.Mappers;
using WorkingWithFiles.Infrastructure.Repositories;
using WorkingWithFiles.Infrastructure.Services;

namespace WorkingWithFiles.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<FilesDbContext>((_, options) =>
        { 
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();
        services.AddScoped<ISampleFileService, SampleFileService>();

        services.AddSingleton<IMapper, LineMapper>();
        services.AddSingleton<ISalesOrderFactory, SalesOrderFactory>();
    }
}