using System.ComponentModel.DataAnnotations;

namespace ShelfTracker.Dtos.Requests;

public class CreateBookRequest
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string Description { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Publish date is required.")]
    public DateTime PublishDate { get; set; }
    
    [Required(ErrorMessage = "At least one author is required.")]
    [MinLength(1, ErrorMessage = "At least one author is required.")]
    public List<String> Authors { get; set; } = new List<String>();
}