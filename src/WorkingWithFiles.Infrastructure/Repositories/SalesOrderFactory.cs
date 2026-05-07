using Bogus;
using WorkingWithFiles.Application.Dtos;
using WorkingWithFiles.Application.Interfaces;

namespace WorkingWithFiles.Infrastructure.Repositories;

public class SalesOrderFactory : ISalesOrderFactory
{
    // Global atomic counter for Id to preserve a single increasing sequence across threads
    private static long _globalId;

    // Thread-local Faker instance so each thread has its own Random and internal state
    private static readonly ThreadLocal<Faker<SalesOrderDto>> _faker =
        new(() =>
        {
            var f = new Faker<SalesOrderDto>();

            f.RuleFor(s => s.Id, _ => (int)Interlocked.Increment(ref _globalId))
             .RuleFor(s => s.RevisionNumber, faker => (byte)faker.Random.Int(0, 3))
             .RuleFor(s => s.OrderDate, faker => faker.Date.Past())
             .RuleFor(s => s.DueDate, (faker, s) => s.OrderDate.AddDays(faker.Random.Int(3, 14)))
             .RuleFor(s => s.Status, faker => faker.Random.Byte(1, 5))
             .RuleFor(s => s.OnlineOrderFlag, faker => faker.Random.Bool())
             .RuleFor(s => s.SalesOrderNumber, faker => $"SO{faker.Random.Int(1000, 9999)}")
             .RuleFor(s => s.ShipMethod, faker => faker.PickRandom("CARGO TRANSPORT 5", "AIR EXPRESS", "GROUND DELIVERY"))
             .RuleFor(s => s.SubTotal, faker => faker.Finance.Amount(500, 10000))
             .RuleFor(s => s.Freight, faker => faker.Finance.Amount(20, 200))
             .RuleFor(s => s.TaxAmt, (_, s) => Math.Round((s.SubTotal + s.Freight) * 0.15m, 2))
             .RuleFor(s => s.TotalDue, (_, s) => s.SubTotal + s.Freight + s.TaxAmt)
             .RuleFor(s => s.RowGuid, _ => Guid.NewGuid())
             .RuleFor(s => s.ModifiedDate, _ => DateTime.UtcNow)
             .RuleFor(s => s.ShipDate, faker => faker.Date.Future())
             .RuleFor(s => s.PurchaseOrderNumber, faker => $"PO{faker.Random.Int(100000, 999999)}")
             .RuleFor(s => s.AccountNumber, faker => $"ACCT{faker.Random.Int(10000, 99999)}")
             .RuleFor(s => s.CreditCardApprovalCode, faker => faker.Random.AlphaNumeric(8).ToUpper())
             .RuleFor(s => s.Comment, faker => faker.Lorem.Sentence());

            return f;
        });

    public SalesOrderDto CreateFakeDto()
    {
        // Use the thread-local Faker instance
        return _faker.Value is not null
            ? _faker.Value.Generate()
            : new SalesOrderDto();
    }
}
