using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShelfTracker.Data;
using ShelfTracker.Dtos.Requests;
using ShelfTracker.Dtos.Responses;
using ShelfTracker.Entities;
using ShelfTracker.Extensions;
using ShelfTracker.Middleware;

namespace ShelfTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public BooksController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
    {
        var books = await _context.Books
            .Where(b => !b.IsDeleted)
            .ToListAsync();
        
        var bookResponses = _mapper.Map<List<BookResponse>>(books);
        return Ok(bookResponses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetBook(int id)
    {
        var book = await _context.Books
            .Where(b => b.Id == id && !b.IsDeleted)
            .FirstOrDefaultAsync();

        if (book == null)
        {
            throw new NotFoundException($"Book with the specified ID {id} not found.");
        }

        var bookResponse = _mapper.Map<BookResponse>(book);
        return Ok(bookResponse);
    }

    [HttpPost]
    public async Task<ActionResult<BookResponse>> CreateBook(CreateBookRequest request)
    {
        ModelState.ThrowIfInvalid();
        
        var book = _mapper.Map<Book>(request);
        book.CreatedAt = DateTime.UtcNow;
        book.UpdatedAt = DateTime.UtcNow;
        
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        await TrackBookCreation(book);
        await _context.SaveChangesAsync();

        var bookResponse = _mapper.Map<BookResponse>(book);
        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, bookResponse);
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<BookResponse>> UpdateBook(int id, UpdateBookRequest request)
    {
        ModelState.ThrowIfInvalid();

        var existingBook = await _context.Books
            .Where(b => b.Id == id && !b.IsDeleted)
            .FirstOrDefaultAsync();

        if (existingBook == null)
        {
            throw new NotFoundException($"Book with ID {id} not found");
        }

        var newBookData = _mapper.Map<Book>(request);
        newBookData.Id = id; 
        
        await TrackChanges(existingBook, newBookData);

        existingBook.Title = request.Title;
        existingBook.Description = request.Description;
        existingBook.PublishDate = request.PublishDate;
        existingBook.Authors = request.Authors;
        existingBook.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    
        return Ok(_mapper.Map<BookResponse>(existingBook));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteBook(int id)
    {
        var book = await _context.Books
            .Where(b => b.Id == id && !b.IsDeleted)
            .FirstOrDefaultAsync();

        if (book == null)
        {
            throw new NotFoundException($"Book with the specified ID {id} not found.");
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