using WorkingWithFiles.Infrastructure.Repositories;

namespace WorkingWithFiles.Tests;

public class FakeDataGeneratorTests
{
    private readonly SalesOrderFactory _sut = new();

    [Fact]
    public void CreateFakeSaleOrder_Should_ReturnFakeSaleOrder()
    {
        // Arrange && Act
        var fakeSaleOrder = _sut.CreateFakeDto();
        
        // Assert
        Assert.NotNull(fakeSaleOrder);
    }
}
