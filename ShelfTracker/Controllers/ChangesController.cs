using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShelfTracker.Data;
using ShelfTracker.Services;

namespace ShelfTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChangesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IChangeHistoryService _changeHistoryService;
    
    public ChangesController(ApplicationDbContext context, IChangeHistoryService changeHistoryService)
    {
        _context = context;
        _changeHistoryService = changeHistoryService;
    }

    [HttpGet]
    public async Task<ActionResult<object>> GetChanges(
        [FromQuery] int? bookId = null,
        [FromQuery] string? changeType = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortBy = "ChangedAt",
        [FromQuery] string sortDirection = "desc",
        [FromQuery] string? groupBy = null)
    {
        var parameters = new ChangeHistoryQueryParameters
        {
            BookId = bookId,
            ChangeType = changeType,
            FromDate = fromDate,
            ToDate = toDate,
            Page = page,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDirection = sortDirection,
            GroupBy = groupBy
        };

        if (!string.IsNullOrEmpty(groupBy))
        {
            var groupedResult = await _changeHistoryService.GetGroupedChangesAsync(parameters);
            return Ok(groupedResult);
        }

        var result = await _changeHistoryService.GetChangesAsync(parameters);
        return Ok(result);
    }

    [HttpGet("book/{bookId}")]
    public async Task<ActionResult<object>> GetBookChanges(int bookId)
    {
        var changes = await _context.ChangeHistories
            .Where(c => c.BookId == bookId)
            .OrderByDescending(c => c.ChangedAt)
            .Select(c => new
            {
                c.Id,
                c.ChangeType,
                c.Description,
                c.OldValue,
                c.NewValue,
                c.FieldName,
                c.ChangedAt
            })
            .ToListAsync();

        return Ok(changes);
    }
}