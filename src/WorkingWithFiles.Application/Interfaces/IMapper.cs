using WorkingWithFiles.Application.Dtos;

namespace WorkingWithFiles.Application.Interfaces;

public interface IMapper
{
    SalesOrderCsvDto MapLine(string[] parts);
}