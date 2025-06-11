using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShelfTracker.Data;
using ShelfTracker.Entities;

namespace ShelfTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    
    public BooksController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
    {
        var books = await _context.Books
            .Where(b => !b.IsDeleted)
            .ToListAsync();
        
        return Ok(books);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetBook(int id)
    {
        var book = await _context.Books
            .Where(b => b.Id == id && !b.IsDeleted)
            .FirstOrDefaultAsync();

        if (book == null)
        {
            return NotFound();
        }

        return Ok(book);
    }

    [HttpPost]
    public async Task<ActionResult<Book>> CreateBook(Book book)
    {
        book.CreatedAt = DateTime.UtcNow;
        book.UpdatedAt = DateTime.UtcNow;
        
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        await TrackBookCreation(book);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<Book>> UpdateBook(int id, Book book)
    {
        if (id != book.Id)
        {
            return BadRequest();
        }

        var existingBook = await _context.Books
            .Where(b => b.Id == id && !b.IsDeleted)
            .FirstOrDefaultAsync();

        if (existingBook == null)
        {
            return NotFound();
        }

        await TrackChanges(existingBook, book);

        existingBook.Title = book.Title;
        existingBook.Description = book.Description;
        existingBook.PublishDate = book.PublishDate;
        existingBook.Authors = book.Authors;
        existingBook.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(existingBook);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteBook(int id)
    {
        var book = await _context.Books
            .Where(b => b.Id == id && !b.IsDeleted)
            .FirstOrDefaultAsync();

        if (book == null)
        {
            return NotFound();
        }

        await TrackBookDeletion(book);

        // Soft delete
        book.IsDeleted = true;
        book.DeletedAt = DateTime.UtcNow;
    
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private async Task TrackBookCreation(Book book)
    {
        var change = new ChangeHistory
        {
            BookId = book.Id,
            BookTitle = book.Title,
            ChangeType = ChangeTypes.Created,
            Description = $"Book '{book.Title}' was created",
            OldValue = null,
            NewValue = $"Title: {book.Title}, Authors: {string.Join(", ", book.Authors)}",
            FieldName = "Book"
        };

        _context.ChangeHistories.Add(change);
    }

    private async Task TrackBookDeletion(Book book)
    {
        var change = new ChangeHistory
        {
            BookId = book.Id,
            BookTitle = book.Title,
            ChangeType = ChangeTypes.Deleted,
            Description = $"Book '{book.Title}' was deleted",
            OldValue = $"Title: {book.Title}, Authors: {string.Join(", ", book.Authors)}",
            NewValue = null,
            FieldName = "Book"
        };

        _context.ChangeHistories.Add(change);
    }

    private async Task TrackChanges(Book oldBook, Book newBook)
    {
        var changes = new List<ChangeHistory>();

        if (oldBook.Title != newBook.Title)
        {
            changes.Add(new ChangeHistory
            {
                BookId = oldBook.Id,
                BookTitle = oldBook.Title,
                ChangeType = ChangeTypes.TitleChanged,
                Description = $"Title was changed from '{oldBook.Title}' to '{newBook.Title}'",
                OldValue = oldBook.Title,
                NewValue = newBook.Title,
                FieldName = "Title"
            });
        }

        if (oldBook.Description != newBook.Description)
        {
            changes.Add(new ChangeHistory
            {
                BookId = oldBook.Id,
                BookTitle = oldBook.Title,
                ChangeType = ChangeTypes.DescriptionChanged,
                Description = $"Description was changed from '{oldBook.Description}' to '{newBook.Description}'",
                OldValue = oldBook.Description,
                NewValue = newBook.Description,
                FieldName = "Description"
            });
        }

        var oldAuthors = string.Join(", ", oldBook.Authors);
        var newAuthors = string.Join(", ", newBook.Authors);
        
        if (oldAuthors != newAuthors)
        {
            changes.Add(new ChangeHistory
            {
                BookId = oldBook.Id,
                BookTitle = oldBook.Title,
                ChangeType = ChangeTypes.AuthorsChanged,
                Description = $"Authors were changed from '{oldAuthors}' to '{newAuthors}'",
                OldValue = oldAuthors,
                NewValue = newAuthors,
                FieldName = "Authors"
            });
        }

        if (changes.Any())
        {
            _context.ChangeHistories.AddRange(changes);
        }
    }
}