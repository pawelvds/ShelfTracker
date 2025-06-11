namespace ShelfTracker.Entities;

public class ChangeHistory
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string ChangeType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    
    public Book Book { get; set; } = null!;
}

public static class ChangeTypes
{
    public const string Created = "Created";
    public const string TitleChanged = "TitleChanged";
    public const string DescriptionChanged = "DescriptionChanged";
    public const string AuthorsChanged = "AuthorsChanged";
    public const string Deleted = "Deleted";
}