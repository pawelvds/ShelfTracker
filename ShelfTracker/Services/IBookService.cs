using ShelfTracker.Dtos.Requests;
using ShelfTracker.Dtos.Responses;

namespace ShelfTracker.Services;

public interface IBookService
{
    Task<List<BookResponse>> GetBooksAsync();
    Task<BookResponse?> GetBookByIdAsync(int id);
    Task<BookResponse> CreateBookAsync(CreateBookRequest request);
    Task<BookResponse> UpdateBookAsync(int id, UpdateBookRequest request);
    Task DeleteBookAsync(int id);
}