using Microsoft.EntityFrameworkCore;

namespace XAM.Models;

public class XamDbContext : DbContext
{
    public XamDbContext(DbContextOptions<XamDbContext> options) : base(options) { }

    public DbSet<DataHolder> DataHoldersTable { get; set; }
    public DbSet<Exam> ExamsTable { get; set; }
    public DbSet<Flashcard> FlashcardsTable { get; set; }
    public DbSet<StatisticsHolder> StatisticsTable { get; set; }

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

        modelBuilder.Entity<DataHolder>()
            .HasOne(dataHolder => dataHolder.Statistics)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade)
            .HasForeignKey<StatisticsHolder>(statisticsHolder => statisticsHolder.StatisticsId);
    }

    public bool SaveToDatabase(DataHolder newData)
    {
        try
        {
            DeleteAndReplaceRow(newData);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occured when saving to the database: {ex.Message}");
            return false;
        }

        return true;
    }

    private void DeleteAndReplaceRow(DataHolder newData)
    {
        var existingData = DataHoldersTable.FirstOrDefault();
        if (existingData != null)
        {
            DataHoldersTable.Remove(existingData);
            SaveChanges();
        }

        DataHoldersTable.Add(newData);
        SaveChanges();
    }
}
