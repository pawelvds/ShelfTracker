using Microsoft.EntityFrameworkCore;
using ShelfTracker.Entities;

namespace ShelfTracker.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
    public DbSet<Book> Books { get; set; }
    public DbSet<ChangeHistory> ChangeHistories { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Authors)
                .HasConversion(
                    v => string.Join(';', v),
                    v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()
                );
        });
        
        modelBuilder.Entity<ChangeHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ChangeType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.BookTitle).HasMaxLength(200);
            entity.Property(e => e.FieldName).HasMaxLength(50);
    
            entity.HasOne(e => e.Book)
                .WithMany()
                .HasForeignKey(e => e.BookId);
        });
        
        base.OnModelCreating(modelBuilder);
    }
}