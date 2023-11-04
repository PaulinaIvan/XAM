namespace XAM.Models;

public class DataHolder
{
    public List<Exam> Exams { get; set; } = new();

    public int LifetimeCreatedExamsCounter { get; set; }
    public int LifetimeCreatedFlashcardsCounter { get; set; }

    public DailyAchievements TodaysAchievements { get; set; } = new(0, 0, 0, 0);
    public string TodaysCocktail { get; set; } = "No cocktail yet";
}