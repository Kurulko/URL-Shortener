using System.Linq.Dynamic.Core;
using System.Reflection;

namespace URL_ShortenerAPI.Data.Results;

public class ApiResult<T>
{
    public IList<T> Data { get; init;  }
    public int PageIndex { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
    public bool HasPreviousPage => PageIndex > 0;
    public bool HasNextPage => (PageIndex + 1) < TotalPages;
    public string? SortColumn { get; set; }
    public string? SortOrder { get; set; }
    public string? FilterColumn { get; set; }
    public string? FilterQuery { get; set; }

    private ApiResult(
        IList<T> data,
        int count,
        int pageIndex,
        int pageSize,
        string? sortColumn,
        string? sortOrder,
        string? filterColumn,
        string? filterQuery)
    {
        Data = data;
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalCount = count;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        SortColumn = sortColumn;
        SortOrder = sortOrder;
        FilterColumn = filterColumn;
        FilterQuery = filterQuery;
    }

    public static async Task<ApiResult<T>> CreateAsync(
        IQueryable<T> source,
        int pageIndex,
        int pageSize,
        string? sortColumn = null,
        string? sortOrder = null,
        string? filterColumn = null,
        string? filterQuery = null)
    {
        if (!string.IsNullOrEmpty(filterColumn) && !string.IsNullOrEmpty(filterQuery) && IsValidProperty(filterColumn))
        {
            source = source.Where(
                string.Format("{0}.ToLower().Contains(@0.ToLower())", filterColumn),
                filterQuery);
        }

        int count = source.Count();

        if (!string.IsNullOrEmpty(sortColumn) && IsValidProperty(sortColumn))
        {
            bool result = isValidSortOrder(sortOrder);

            if (!result)
                sortOrder = "ASC";

            source = source.OrderBy($"{sortColumn} {sortOrder}");
        }

        source = source
            .Skip(pageIndex * pageSize)
            .Take(pageSize);

        var data = source.ToList();

        var apiResult = new ApiResult<T>(
            data,
            count,
            pageIndex,
            pageSize,
            sortColumn,
            sortOrder,
            filterColumn,
            filterQuery
        );
        return await Task.FromResult(apiResult);
    }

    static bool IsValidProperty(string propertyName)
    {
        var prop = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (prop is null)
            throw new NotSupportedException($"ERROR: Property '{propertyName}' doesn't exist.");

        return prop != null;
    }

    static bool isValidSortOrder(string? sortOrder)
    {
        if (string.IsNullOrEmpty(sortOrder))
            return false;

        var possibleSortOrders = new List<string>() { "ascending", "asc" , "descending", "desc" };
        return possibleSortOrders.Any(so => so == sortOrder);
    }
}