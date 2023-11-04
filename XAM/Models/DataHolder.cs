using System.ComponentModel.DataAnnotations;

namespace XAM.Models;

public class DataHolder
{
    [Key] public int DataHolderId { get; set; }

    public List<Exam> Exams { get; set; } = new();

    public int LifetimeCreatedExamsCounter { get; set; }
    public int LifetimeCreatedFlashcardsCounter { get; set; }

    public string TodaysCocktail { get; set; } = "No cocktail yet";
}