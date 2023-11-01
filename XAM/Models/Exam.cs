public class Exam : IComparable<Exam>
{
    private string NameField;
    public string Name { get { return NameField; } set { NameField = value; ValidNameCheck(value); } }
    public DateTime Date { get; set; }
    public List<Flashcard> Flashcards { get; set; } // List of flashcards
    public int ChallengeHighscore { get; set; } = 0;

    public Exam(string name, DateTime date)
    {
        NameField = name;
        Date = date;
        Flashcards = new List<Flashcard>(); // Initialize the list of flashcards
    }

    private void ValidNameCheck(string newName)
    {
        try
        {
            if (!newName.IsMadeOfLettersNumbersAndSpaces() || newName == "")
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

public struct Flashcard
{
    public string FrontText { get; set; }
    public string BackText { get; set; }

    public Flashcard(string frontText, string backText)
    {
        FrontText = frontText;
        BackText = backText;
    }
}