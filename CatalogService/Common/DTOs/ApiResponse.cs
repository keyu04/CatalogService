namespace CatalogService.Common.DTOs;

public class PagedResultDto<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNext => Page < TotalPages;
    public bool HasPrevious => Page > 1;
}

public class ApiResponse<T>
{
    public bool status { get; set; }

    public string Message { get; set; } = string.Empty;

    public string? Code { get; set; }

    public T? Data { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}