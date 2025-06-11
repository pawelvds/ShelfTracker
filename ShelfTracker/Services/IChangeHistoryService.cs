namespace ShelfTracker.Services;

public interface IChangeHistoryService
{
    Task<object> GetChangesAsync(ChangeHistoryQueryParameters parameters);
    Task<object> GetGroupedChangesAsync(ChangeHistoryQueryParameters parameters);
}