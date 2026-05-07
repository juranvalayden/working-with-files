using WorkingWithFiles.Application.Dtos;

namespace WorkingWithFiles.Application.Interfaces;

public interface ISalesOrderFactory
{
    SalesOrderDto CreateFakeDto();
}