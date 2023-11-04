using Microsoft.EntityFrameworkCore;

namespace XAM.Models;

public class XamDbContext : DbContext
{
    public XamDbContext(DbContextOptions<XamDbContext> options) : base(options) { }

    public DbSet<DataHolder> DataHolders { get; set; }
    public DbSet<Exam> Exams { get; set; }
    public DbSet<Flashcard> Flashcards { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DataHolder>()
            .HasMany(dataHolder => dataHolder.Exams)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Exam>()
            .HasMany(exam => exam.Flashcards)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void DeleteAndReplaceRow(DataHolder newData)
    {
        var existingData = DataHolders.FirstOrDefault();
        if (existingData != null)
        {
            DataHolders.Remove(existingData);
            SaveChanges();
        }

        DataHolders.Add(newData);
        SaveChanges();
    }
}
