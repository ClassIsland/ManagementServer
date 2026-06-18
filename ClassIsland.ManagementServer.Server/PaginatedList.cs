using System.Linq.Expressions;
using ClassIsland.ManagementServer.Server.Abstractions.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server;

public class PaginatedList<T>(List<T> items, int count, int pageIndex, int pageSize) where T : IObjectWithTime
{
    public int PageIndex { get; private set; } = pageIndex;
    public int TotalPages { get; private set; } = (int)Math.Ceiling(count / (double)pageSize);

    public int PageSize { get; private set; } = pageSize;

    public int ItemCount { get; private set; } = count;

    public List<T> Items { get; } = items;

    public bool HasPreviousPage => PageIndex > 1;

    public bool HasNextPage => PageIndex < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize
        , bool decreasing = false, bool orderByUpdatedTime = false) 
    {
        if (pageIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageIndex), "Page index should be non-negative");
        }
        
        // Normalize: accept 0-based or 1-based indexing
        var normalizedIndex = pageIndex == 0 ? 1 : pageIndex;
        
        var count = await source.CountAsync();
        var query = source;
        query = decreasing ? query.OrderByDescending(x => orderByUpdatedTime ? x.UpdatedTime : x.CreatedTime) 
            : query.OrderBy(x => orderByUpdatedTime ? x.UpdatedTime : x.CreatedTime);
        var items = await query
            .Skip((normalizedIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return new PaginatedList<T>(items, count, normalizedIndex, pageSize);
    }
    
    public static PaginatedList<T> CreateFromRawList(IList<T> source, int pageIndex, int pageSize)
    {
        if (pageIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageIndex), "Page index should be non-negative");
        }
        
        var normalizedIndex = pageIndex == 0 ? 1 : pageIndex;
        
        var count = source.Count;
        var items = source.Skip((normalizedIndex - 1) * pageSize).Take(pageSize).ToList();
        return new PaginatedList<T>(items, count, normalizedIndex, pageSize);
    }
}