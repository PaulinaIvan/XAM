using Microsoft.EntityFrameworkCore;

namespace XAM.Models;

public class XamDbContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public XamDbContext(DbContextOptions<XamDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

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

    public DataHolder GetDataHolder()
    {
        string? username = _httpContextAccessor.HttpContext?.Session.GetString("CurrentUser");

        DataHolder? foundDataHolder = DataHoldersTable
            .Include(dataHolder => dataHolder.Exams)
                .ThenInclude(exam => exam.Flashcards)
            .Include(dataHolder => dataHolder.Statistics)
            .FirstOrDefault(dataHolder => dataHolder.OwnerUsername == username);

        if (foundDataHolder != null)
        {
            return foundDataHolder;
        }
        else
        {
            DataHolder newDataHolder = new() { OwnerUsername = username };
            DataHoldersTable.Add(newDataHolder);
            SaveChanges();

            return newDataHolder;
        }
    }

    public void SaveToDatabase(DataHolder dataHolderToSave)
    {
        DataHolder? foundDataHolder = DataHoldersTable.FirstOrDefault(dataHolder => dataHolder.DataHolderId == dataHolderToSave.DataHolderId);

        if (foundDataHolder != null)
        {
            Entry(foundDataHolder).State = EntityState.Modified;
            SaveChanges();
        }
        else
        {
            throw new Exception("Somehow the dataholder is gone.");
        }
    }
}
