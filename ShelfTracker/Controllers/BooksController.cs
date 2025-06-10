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

        // Soft delete
        book.IsDeleted = true;
        book.DeletedAt = DateTime.UtcNow;
    
        await _context.SaveChangesAsync();
        return NoContent();
    }
}