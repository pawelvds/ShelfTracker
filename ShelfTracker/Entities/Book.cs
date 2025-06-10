namespace ShelfTracker.Entities;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }
    public List<string> Authors { get; set; } = new List<string>();

    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; } = null;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}