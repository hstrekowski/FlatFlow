namespace FlatFlow.Application.Common.Models;

public record PaginatedResult<T>(List<T> Items, int TotalCount, int Page, int PageSize)
{
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page * PageSize < TotalCount;
}
