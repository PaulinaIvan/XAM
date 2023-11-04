using System.ComponentModel.DataAnnotations;

namespace XAM.Models;

public class Exam : IComparable<Exam>
{
    [Key] public int ExamId { get; set; }
    private string NameField;
    public string Name { get { return NameField; } set { NameField = value; ValidNameCheck(value); } }
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
        NameField = name;
        Date = date;
        Flashcards = new List<Flashcard>(); // Initialize the list of flashcards
    }

    private static void ValidNameCheck(string newName)
    {
        try
        {
            if (!newName.IsValidExamName() || newName == "")
                throw new Exception();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
        }
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