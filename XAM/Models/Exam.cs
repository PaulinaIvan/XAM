using System.ComponentModel.DataAnnotations;

namespace XAM.Models;

public class Exam : IComparable<Exam>
{
    [Key] public int ExamId { get; set; }
    public string Name { get; set; }
    public DateTime Date
    {
        get => _date;
        set => _date = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    private DateTime _date;

    public List<Flashcard> Flashcards { get; set; } // List of flashcards
    public int ChallengeHighscore { get; set; } = 0;

    public Exam(string name, DateTime date)
    {
        Name = name;
        Date = date;
        Flashcards = new List<Flashcard>(); // Initialize the list of flashcards
    }

    public int CompareTo(Exam? other)
    {
        return Name.CompareTo(other?.Name);
    }

    public void DeleteFlashcard(Flashcard flashcard)
    {
        Flashcards.Remove(flashcard);
    }
}