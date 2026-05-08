using WorkingWithFiles.Application.Dtos;
using WorkingWithFiles.Application.Interfaces;

namespace WorkingWithFiles.Infrastructure.Mappers;

public class LineMapper : IMapper
{
    public SalesOrderCsvDto MapLine(string[] parts)
    {
        var salesOrderCsvDto = new SalesOrderCsvDto
        {
            Id = int.Parse(parts[0]),
            RevisionNumber = byte.Parse(parts[1]),
            OrderDate = DateTime.Parse(parts[2]),
            DueDate = DateTime.Parse(parts[3]),
            Status = byte.Parse(parts[4]),
            OnlineOrderFlag = bool.Parse(parts[5]),
            SalesOrderNumber = parts[6],
            ShipMethod = parts[7],
            SubTotal = decimal.Parse(parts[8]),
            TaxAmt = decimal.Parse(parts[9]),
            Freight = decimal.Parse(parts[10]),
            TotalDue = decimal.Parse(parts[11]),
            RowGuid = Guid.Parse(parts[12]),
            ModifiedDate = DateTime.Parse(parts[13]),
            ShipDate = string.IsNullOrWhiteSpace(parts[14]) ? null : DateTime.Parse(parts[14]),
            PurchaseOrderNumber = string.IsNullOrWhiteSpace(parts[15]) ? null : parts[15],
            AccountNumber = string.IsNullOrWhiteSpace(parts[16]) ? null : parts[16],
            CreditCardApprovalCode = string.IsNullOrWhiteSpace(parts[17]) ? null : parts[17],
            Comment = string.IsNullOrWhiteSpace(parts[18]) ? null : parts[18]
        };

        return salesOrderCsvDto;
    }
}