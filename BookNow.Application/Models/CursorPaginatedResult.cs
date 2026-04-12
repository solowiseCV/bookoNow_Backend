namespace BookNow.Application.Models;

public class CursorPaginatedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public string? NextCursor { get; }

    public CursorPaginatedResult(IReadOnlyList<T> items, string? nextCursor)
    {
        Items = items;
        NextCursor = nextCursor;
    }
}
