using WorkingWithFiles.Application.Dtos;
using WorkingWithFiles.Domain.Entities;

namespace WorkingWithFiles.Application.Interfaces;

public interface IMapper
{
    SalesOrderCsvDto MapLine(string[] parts);
    SalesOrder MapDtoToEntity(string[] parts);
}