using ShelfTracker.Data;
using ShelfTracker.Entities;

namespace ShelfTracker;

public static class SeedData
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (context.Books.Any())
            return;

        var books = new List<Book>
        {
            new Book { Title = "The Hobbit", Description = "A fantasy adventure", PublishDate = new DateTime(1937, 9, 21), Authors = new List<string> { "J.R.R. Tolkien" } },
            new Book { Title = "1984", Description = "Dystopian novel", PublishDate = new DateTime(1949, 6, 8), Authors = new List<string> { "George Orwell" } },
            new Book { Title = "To Kill a Mockingbird", Description = "Classic of American literature", PublishDate = new DateTime(1960, 7, 11), Authors = new List<string> { "Harper Lee" } },
            new Book { Title = "The Great Gatsby", Description = "Roaring twenties novel", PublishDate = new DateTime(1925, 4, 10), Authors = new List<string> { "F. Scott Fitzgerald" } },
            new Book { Title = "Harry Potter and the Sorcerer's Stone", Description = "Wizarding adventure", PublishDate = new DateTime(1997, 6, 26), Authors = new List<string> { "J.K. Rowling" } },
            new Book { Title = "The Catcher in the Rye", Description = "Teen angst classic", PublishDate = new DateTime(1951, 7, 16), Authors = new List<string> { "J.D. Salinger" } },
            new Book { Title = "Pride and Prejudice", Description = "Romantic classic", PublishDate = new DateTime(1813, 1, 28), Authors = new List<string> { "Jane Austen" } },
            new Book { Title = "The Lord of the Rings", Description = "Epic fantasy trilogy", PublishDate = new DateTime(1954, 7, 29), Authors = new List<string> { "J.R.R. Tolkien" } },
            new Book { Title = "Animal Farm", Description = "Political satire", PublishDate = new DateTime(1945, 8, 17), Authors = new List<string> { "George Orwell" } },
            new Book { Title = "Brave New World", Description = "Dystopian future", PublishDate = new DateTime(1932, 1, 1), Authors = new List<string> { "Aldous Huxley" } },
            new Book { Title = "Jane Eyre", Description = "Gothic romance", PublishDate = new DateTime(1847, 10, 16), Authors = new List<string> { "Charlotte Brontë" } },
            new Book { Title = "Wuthering Heights", Description = "Tragic love story", PublishDate = new DateTime(1847, 12, 1), Authors = new List<string> { "Emily Brontë" } },
            new Book { Title = "The Da Vinci Code", Description = "Mystery thriller", PublishDate = new DateTime(2003, 3, 18), Authors = new List<string> { "Dan Brown" } },
            new Book { Title = "Good Omens", Description = "Comedy about apocalypse", PublishDate = new DateTime(1990, 5, 1), Authors = new List<string> { "Neil Gaiman", "Terry Pratchett" } },
            new Book { Title = "Moby-Dick", Description = "Whaling adventure", PublishDate = new DateTime(1851, 11, 14), Authors = new List<string> { "Herman Melville" } }
        };

        foreach (var book in books)
        {
            book.CreatedAt = DateTime.UtcNow.AddDays(-new Random().Next(1, 100));
            book.UpdatedAt = book.CreatedAt;
        }

        context.Books.AddRange(books);
        await context.SaveChangesAsync();
    }
}
