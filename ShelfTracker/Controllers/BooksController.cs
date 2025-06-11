using Microsoft.AspNetCore.Mvc;
using ShelfTracker.Dtos.Requests;
using ShelfTracker.Dtos.Responses;
using ShelfTracker.Extensions;
using ShelfTracker.Services;

namespace ShelfTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookResponse>>> GetBooks()
    {
        var books = await _bookService.GetBooksAsync();
        return Ok(books);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookResponse>> GetBook(int id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        return Ok(book);
    }

    [HttpPost]
    public async Task<ActionResult<BookResponse>> CreateBook(CreateBookRequest request)
    {
        ModelState.ThrowIfInvalid();
        
        var book = await _bookService.CreateBookAsync(request);
        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BookResponse>> UpdateBook(int id, UpdateBookRequest request)
    {
        ModelState.ThrowIfInvalid();

        var book = await _bookService.UpdateBookAsync(id, request);
        return Ok(book);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteBook(int id)
    {
        await _bookService.DeleteBookAsync(id);
        return NoContent();
    }
}