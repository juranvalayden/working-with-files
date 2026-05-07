using Bogus;
using WorkingWithFiles.Application.Dtos;
using WorkingWithFiles.Application.Interfaces;
using WorkingWithFiles.Domain.Entities;

namespace WorkingWithFiles.Infrastructure.Repositories;

public class SalesOrderFactory : ISalesOrderFactory
{
    public SalesOrderDto CreateFakeDto()
    {
        Randomizer.Seed = new Random(3897234);

        var faker = new Faker<SalesOrderDto>()
            .RuleFor(s => s.Id, f => f.IndexFaker + 1)
            .RuleFor(s => s.RevisionNumber, f => (byte)f.Random.Int(0, 3))
            .RuleFor(s => s.OrderDate, f => f.Date.Past(1))
            .RuleFor(s => s.DueDate, (f, s) => s.OrderDate.AddDays(f.Random.Int(3, 14)))
            .RuleFor(s => s.Status, f => f.Random.Byte(1, 5))
            .RuleFor(s => s.OnlineOrderFlag, f => f.Random.Bool())
            .RuleFor(s => s.SalesOrderNumber, f => $"SO{f.Random.Int(1000, 9999)}")
            .RuleFor(s => s.ShipMethod, f => f.PickRandom("CARGO TRANSPORT 5", "AIR EXPRESS", "GROUND DELIVERY"))
            .RuleFor(s => s.SubTotal, f => f.Finance.Amount(500, 10000))
            .RuleFor(s => s.Freight, f => f.Finance.Amount(20, 200))
            .RuleFor(s => s.TaxAmt, (f, s) => Math.Round((s.SubTotal + s.Freight) * 0.15m, 2))
            .RuleFor(s => s.TotalDue, (f, s) => s.SubTotal + s.Freight + s.TaxAmt)
            .RuleFor(s => s.RowGuid, f => Guid.NewGuid())
            .RuleFor(s => s.ModifiedDate, f => DateTime.UtcNow)

            // Meaningful fields
            .RuleFor(s => s.ShipDate, f => f.Date.Future(1)) // always a future date
            .RuleFor(s => s.PurchaseOrderNumber, f => $"PO{f.Random.Int(100000, 999999)}") // e.g. PO120384
            .RuleFor(s => s.AccountNumber, f => $"ACCT{f.Random.Int(10000, 99999)}")
            .RuleFor(s => s.CreditCardApprovalCode, f => f.Random.AlphaNumeric(8).ToUpper()) // e.g. "AB12CD34"
            .RuleFor(s => s.Comment, f => f.Lorem.Sentence());

        return faker.Generate();
    }

    public SalesOrder Create()
    {
        const int subTotalMin = 500;
        const int subTotalMax = 10000;
        const int freightMin = 20;
        const int freightMax = 200;

        var subTotal = Random.Shared.NextDouble() * (subTotalMax - subTotalMin) + subTotalMin;
        var freight = Random.Shared.NextDouble() * (freightMax - freightMin) + freightMin;
        var taxAmount = (subTotal + freight) * 0.15;

        return new SalesOrder
        {
            RevisionNumber = 0,
            Status = 5,
            CustomerId = 29847,
            ShipMethod = "CARGO TRANSPORT 5",
            SubTotal = (decimal)Math.Round(subTotal, 2),
            TaxAmount = (decimal)Math.Round(taxAmount, 2),
            Freight = (decimal)Math.Round(freight, 2),
            ShipDate = null,
            PurchaseOrderNumber = null,
            AccountNumber = null,
            ShipToAddressId = null,
            BillToAddressId = null,
            CreditCardApprovalCode = null,
            Comment = null
        };
    }

    public SalesOrder CreateWithBogus()
    {
        Randomizer.Seed = new Random(3897234);

        var faker = new Faker<SalesOrder>()
            .RuleFor(s => s.Id, f => f.IndexFaker + 1)
            .RuleFor(s => s.RevisionNumber, f => (byte)0)
            .RuleFor(s => s.Status, f => f.Random.Byte(1, 5)) // ✅ use Random.Byte
            .RuleFor(s => s.CustomerId, f => f.Random.Int(1000, 50000))
            .RuleFor(s => s.ShipMethod, f => f.PickRandom("CARGO TRANSPORT 5", "AIR EXPRESS", "GROUND DELIVERY"))
            .RuleFor(s => s.OrderDate, f => f.Date.Past(1))
            .RuleFor(s => s.DueDate, (f, s) => s.OrderDate.AddDays(f.Random.Int(3, 14)))
            .RuleFor(s => s.ShipDate, (f, s) => f.Random.Bool() ? s.OrderDate.AddDays(f.Random.Int(1, 7)) : null)
            .RuleFor(s => s.PurchaseOrderNumber, f => f.Random.Bool() ? $"PO{f.Random.Int(1000, 9999)}" : null)
            .RuleFor(s => s.AccountNumber, f => f.Random.Bool() ? $"ACCT{f.Random.Int(10000, 99999)}" : null)
            .RuleFor(s => s.CreditCardApprovalCode, f => f.Random.Bool() ? f.Finance.CreditCardNumber() : null)
            .RuleFor(s => s.Comment, f => f.Random.Bool() ? f.Lorem.Sentence() : null)
            .RuleFor(s => s.SubTotal, f => f.Finance.Amount(500, 10000))
            .RuleFor(s => s.Freight, f => f.Finance.Amount(20, 200))
            .RuleFor(s => s.TaxAmt, (f, s) => Math.Round((s.SubTotal + s.Freight) * 0.15m, 2))
            .RuleFor(s => s.TotalDue, (f, s) => s.SubTotal + s.Freight + s.TaxAmt)
            .RuleFor(s => s.RowGuid, f => Guid.NewGuid())
            .RuleFor(s => s.ModifiedDate, f => DateTime.UtcNow);

        return faker.Generate();
    }
}