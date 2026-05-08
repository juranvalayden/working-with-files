using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

        // Use pooled factory for background/bulk work
        services.AddPooledDbContextFactory<FilesDbContext>(options =>
        {
            options.UseSqlServer(connectionString);

            // Reduce SQL logging noise: only warnings/errors
            options.LogTo(Console.WriteLine, [DbLoggerCategory.Database.Command.Name], LogLevel.Warning);
        });

        services.AddScoped<IBulkInsert, BulkCopySalesOrderInserter>();

        // services.AddScoped<IBulkInsert, EfSalesOrderInserter>();
        // services.AddScoped<IBulkInsert, RawSqlSalesOrderInserter>();
        // services.AddScoped<EfSalesOrderInserter>();
        // services.AddScoped<RawSqlSalesOrderInserter>();
        // services.AddScoped<BulkCopySalesOrderInserter>();

        services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();
        services.AddScoped<ISampleFileService, SampleFileService>();

        // Stateless helpers can be singletons
        services.AddSingleton<IMapper, LineMapper>();
        services.AddSingleton<ISalesOrderFactory, SalesOrderFactory>();
    }
}
