namespace ShelfTracker.Services;

public class ChangeHistoryQueryParameters
{
    public int? BookId { get; set; }
    public string? ChangeType { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = "ChangedAt";
    public string SortDirection { get; set; } = "desc";
    public string? GroupBy { get; set; }
}