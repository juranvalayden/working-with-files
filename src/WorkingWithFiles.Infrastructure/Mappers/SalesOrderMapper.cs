using WorkingWithFiles.Application.Dtos;
using WorkingWithFiles.Domain.Entities;

namespace WorkingWithFiles.Infrastructure.Mappers;

public class SalesOrderMapper
{
    public static SalesOrder MapDtoToEntity(SalesOrderCsvDto dto)
    {
        return new SalesOrder
        {
            RevisionNumber = dto.RevisionNumber,
            OrderDate = dto.OrderDate,
            DueDate = dto.DueDate,
            Status = dto.Status,
            OnlineOrderFlag = dto.OnlineOrderFlag,
            SalesOrderNumber = dto.SalesOrderNumber,
            ShipMethod = dto.ShipMethod,
            SubTotal = dto.SubTotal,
            TaxAmt = dto.TaxAmt,
            Freight = dto.Freight,
            TotalDue = dto.TotalDue,
            RowGuid = dto.RowGuid,
            ModifiedDate = dto.ModifiedDate,
            ShipDate = dto.ShipDate,
            PurchaseOrderNumber = dto.PurchaseOrderNumber,
            AccountNumber = dto.AccountNumber,
            CreditCardApprovalCode = dto.CreditCardApprovalCode,
            Comment = dto.Comment
        };
    }

}