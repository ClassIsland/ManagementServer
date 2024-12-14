namespace ClassIsland.ManagementServer.Server.Extensions;

public static class PaginatedListExtensions
{
    public static async Task<PaginatedList<T>> ToPaginatedListAsync<T>(this IQueryable<T> queryable, int pageIndex, int pageSize)
    {
        return await PaginatedList<T>.CreateAsync(queryable, pageIndex, pageSize);
    } 
}