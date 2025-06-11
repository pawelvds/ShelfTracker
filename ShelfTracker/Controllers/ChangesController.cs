using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShelfTracker.Data;
using ShelfTracker.Entities;

namespace ShelfTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChangesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    
    public ChangesController(ApplicationDbContext context)
    {
        _context = context;
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
        [FromQuery] string sortDirection = "desc")
    {
        var query = _context.ChangeHistories.AsQueryable();

        if (bookId.HasValue)
            query = query.Where(c => c.BookId == bookId.Value);

        if (!string.IsNullOrEmpty(changeType))
            query = query.Where(c => c.ChangeType == changeType);

        if (fromDate.HasValue)
            query = query.Where(c => c.ChangedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(c => c.ChangedAt <= toDate.Value);

        query = sortBy.ToLower() switch
        {
            "changedat" => sortDirection.ToLower() == "asc" 
                ? query.OrderBy(c => c.ChangedAt)
                : query.OrderByDescending(c => c.ChangedAt),
            "changetype" => sortDirection.ToLower() == "asc" 
                ? query.OrderBy(c => c.ChangeType)
                : query.OrderByDescending(c => c.ChangeType),
            _ => query.OrderByDescending(c => c.ChangedAt)
        };

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        var changes = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
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

        return Ok(new
        {
            Data = changes,
            Pagination = new
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            },
            Filters = new
            {
                AvailableChangeTypes = new[] { 
                    ChangeTypes.Created, 
                    ChangeTypes.TitleChanged, 
                    ChangeTypes.DescriptionChanged, 
                    ChangeTypes.AuthorsChanged, 
                    ChangeTypes.Deleted 
                }
            }
        });
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