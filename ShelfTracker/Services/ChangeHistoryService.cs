using Microsoft.EntityFrameworkCore;
using ShelfTracker.Data;
using ShelfTracker.Entities;

namespace ShelfTracker.Services;

public class ChangeHistoryService : IChangeHistoryService
{
    private readonly ApplicationDbContext _context;

    public ChangeHistoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<object> GetChangesAsync(ChangeHistoryQueryParameters parameters)
    {
        var query = BuildBaseQuery(parameters);

        query = parameters.SortBy.ToLower() switch
        {
            "changedat" => parameters.SortDirection.ToLower() == "asc" 
                ? query.OrderBy(c => c.ChangedAt)
                : query.OrderByDescending(c => c.ChangedAt),
            "changetype" => parameters.SortDirection.ToLower() == "asc" 
                ? query.OrderBy(c => c.ChangeType)
                : query.OrderByDescending(c => c.ChangeType),
            _ => query.OrderByDescending(c => c.ChangedAt)
        };

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalItems / parameters.PageSize);

        var changes = await query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .Select(c => new
            {
                c.Id,
                c.BookId,
                c.BookTitle,
                c.ChangeType,
                c.Description,
                c.OldValue,
                c.NewValue,
                c.FieldName,
                c.ChangedAt
            })
            .ToListAsync();

        return new
        {
            Data = changes,
            Pagination = new
            {
                CurrentPage = parameters.Page,
                PageSize = parameters.PageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            }
        };
    }

    public async Task<object> GetGroupedChangesAsync(ChangeHistoryQueryParameters parameters)
    {
        var query = BuildBaseQuery(parameters);

        var groupedData = parameters.GroupBy!.ToLower() switch
        {
            "day" => await GroupByDay(query),
            "week" => await GroupByWeek(query),
            "month" => await GroupByMonth(query),
            "changetype" => await GroupByChangeType(query),
            _ => throw new ArgumentException($"Invalid groupBy value: {parameters.GroupBy}")
        };

        var totalGroups = groupedData.Count;
        var totalPages = (int)Math.Ceiling((double)totalGroups / parameters.PageSize);
        
        var pagedGroups = groupedData
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToList();

        return new
        {
            Groups = pagedGroups,
            Pagination = new
            {
                CurrentPage = parameters.Page,
                PageSize = parameters.PageSize,
                TotalItems = totalGroups,
                TotalPages = totalPages
            },
            GroupBy = parameters.GroupBy
        };
    }

    private IQueryable<ChangeHistory> BuildBaseQuery(ChangeHistoryQueryParameters parameters)
    {
        var query = _context.ChangeHistories.AsQueryable();

        if (parameters.BookId.HasValue)
            query = query.Where(c => c.BookId == parameters.BookId.Value);

        if (!string.IsNullOrEmpty(parameters.ChangeType))
            query = query.Where(c => c.ChangeType == parameters.ChangeType);

        if (parameters.FromDate.HasValue)
            query = query.Where(c => c.ChangedAt >= parameters.FromDate.Value);

        if (parameters.ToDate.HasValue)
            query = query.Where(c => c.ChangedAt <= parameters.ToDate.Value);

        return query;
    }

private async Task<List<object>> GroupByDay(IQueryable<ChangeHistory> query)
    {
        var grouped = await query
            .GroupBy(c => c.ChangedAt.Date)
            .Select(g => new
            {
                GroupKey = g.Key.ToString("yyyy-MM-dd"),
                GroupName = g.Key.ToString("MMMM dd, yyyy"),
                Count = g.Count(),
                Changes = g.OrderByDescending(c => c.ChangedAt).Select(c => new
                {
                    c.Id,
                    c.BookId,
                    c.BookTitle,
                    c.ChangeType,
                    c.Description,
                    c.ChangedAt
                }).ToList()
            })
            .OrderByDescending(g => g.GroupKey)
            .ToListAsync();

        return grouped.Cast<object>().ToList();
    }
    
    private async Task<List<object>> GroupByWeek(IQueryable<ChangeHistory> query)
    {
        var grouped = await query
            .ToListAsync(); 

        var weekGroups = grouped
            .GroupBy(c => GetWeekStart(c.ChangedAt))
            .Select(g => new
            {
                GroupKey = g.Key.ToString("yyyy-MM-dd"),
                GroupName = $"Week of {g.Key:MMMM dd, yyyy}",
                Count = g.Count(),
                Changes = g.OrderByDescending(c => c.ChangedAt).Select(c => new
                {
                    c.Id,
                    c.BookId,
                    c.BookTitle,
                    c.ChangeType,
                    c.Description,
                    c.ChangedAt
                }).ToList()
            })
            .OrderByDescending(g => g.GroupKey)
            .ToList();

        return weekGroups.Cast<object>().ToList();
    }
    
    private async Task<List<object>> GroupByMonth(IQueryable<ChangeHistory> query)
    {
        var grouped = await query
            .GroupBy(c => new { c.ChangedAt.Year, c.ChangedAt.Month })
            .Select(g => new
            {
                GroupKey = $"{g.Key.Year}-{g.Key.Month:D2}",
                GroupName = $"{new DateTime(g.Key.Year, g.Key.Month, 1):MMMM yyyy}",
                Count = g.Count(),
                Changes = g.OrderByDescending(c => c.ChangedAt).Select(c => new
                {
                    c.Id,
                    c.BookId,
                    c.BookTitle,
                    c.ChangeType,
                    c.Description,
                    c.ChangedAt
                }).ToList()
            })
            .OrderByDescending(g => g.GroupKey)
            .ToListAsync();

        return grouped.Cast<object>().ToList();
    }
    
    private async Task<List<object>> GroupByChangeType(IQueryable<ChangeHistory> query)
    {
        var grouped = await query
            .GroupBy(c => c.ChangeType)
            .Select(g => new
            {
                GroupKey = g.Key,
                GroupName = g.Key,
                Count = g.Count(),
                Changes = g.OrderByDescending(c => c.ChangedAt).Select(c => new
                {
                    c.Id,
                    c.BookId,
                    c.BookTitle,
                    c.ChangeType,
                    c.Description,
                    c.ChangedAt
                }).ToList()
            })
            .OrderBy(g => g.GroupKey)
            .ToListAsync();

        return grouped.Cast<object>().ToList();
    }
    
    private static DateTime GetWeekStart(DateTime date)
    {
        var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-1 * diff).Date;
    }
}