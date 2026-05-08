using Microsoft.EntityFrameworkCore;
using WorkingWithFiles.Domain.Entities;

namespace WorkingWithFiles.Infrastructure.Repositories;

public class FilesDbContext(DbContextOptions<FilesDbContext> options) : DbContext(options)
{
    public DbSet<SalesOrder> SalesOrders { get; set; } 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new SalesOrderConfiguration());
    }
}