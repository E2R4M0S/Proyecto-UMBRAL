using System.Collections.Generic;

namespace Umbral.Domain.Primitives;

/// <summary>
/// Generic paginated result for queries that return paged data.
/// Lives in Domain because repository contracts (IMissionRepository) need it as return type.
/// </summary>
public record PaginatedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages { get; }

    private PaginatedResult()
    {
        Items = Array.Empty<T>();
    }

    public PaginatedResult(IReadOnlyList<T> items, int page, int pageSize, int totalCount)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = pageSize > 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 0;
    }

    /// <summary>
    /// Creates an empty result (useful for testing or no-match scenarios).
    /// </summary>
    public static PaginatedResult<T> Empty => new();
}
