public class Exam : IComparable<Exam> // 1.1. Creating and using your own class.
{
    // 2.1. Property usage in class.
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public List<Flashcard> Flashcards { get; set; } // List of flashcards

    public Exam(string name, DateTime date)
    {
        Name = name;
        Date = date;
        Flashcards = new List<Flashcard>(); // Initialize the list of flashcards
    }

    public int CompareTo(Exam? other) // 10. Implement at least one of the standard .NET interfaces (IEnumerable, IComparable, IComparer, IEquatable, IEnumerator, etc.).
    {
        return Name.CompareTo(other?.Name);
    }

    public void DeleteFlashcard(Flashcard flashcard)
    {
        Flashcards.Remove(flashcard);
    }
}

public struct Flashcard // 1.2. Creating and using your own struct.
{
    // 2.2. Property usage in struct.
    public string FrontText { get; set; }
    public string BackText { get; set; }

    public Flashcard(string frontText, string backText)
    {
        FrontText = frontText;
        BackText = backText;
    }
}