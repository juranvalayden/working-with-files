using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkingWithFiles.Domain.Entities;

namespace WorkingWithFiles.Infrastructure.Repositories;

public class SalesOrderConfiguration : IEntityTypeConfiguration<SalesOrder>
{
    public void Configure(EntityTypeBuilder<SalesOrder> builder)
    {
        builder.ToTable("SalesOrders");

        // Auto-generated primary key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .HasColumnType("int")
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.RevisionNumber)
            .HasColumnType("tinyint")
            .IsRequired();

        builder.Property(x => x.OrderDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(x => x.DueDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnType("tinyint")
            .IsRequired();

        builder.Property(x => x.OnlineOrderFlag)
            .HasColumnType("bit")
            .IsRequired();

        builder.Property(x => x.SalesOrderNumber)
            .HasColumnType("nvarchar(50)")
            .IsRequired();

        builder.Property(x => x.ShipMethod)
            .HasColumnType("nvarchar(50)")
            .IsRequired();

        builder.Property(x => x.SubTotal)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.TaxAmt)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.Freight)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.TotalDue)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.RowGuid)
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(x => x.ModifiedDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(x => x.ShipDate)
            .HasColumnType("date")
            .IsRequired(false);

        builder.Property(x => x.PurchaseOrderNumber)
            .HasColumnType("nvarchar(25)")
            .IsRequired(false);

        builder.Property(x => x.AccountNumber)
            .HasColumnType("nvarchar(25)")
            .IsRequired(false);

        builder.Property(x => x.CreditCardApprovalCode)
            .HasColumnType("nvarchar(25)")
            .IsRequired(false);

        builder.Property(x => x.Comment)
            .HasColumnType("nvarchar(max)")
            .IsRequired(false);
    }
}
