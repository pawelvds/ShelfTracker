namespace ShelfTracker.Dtos.Responses;

public class BookResponse
{
    public int BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }
    public List<string> Authors { get; set; } = new List<string>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}