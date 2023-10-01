public class Exam
{
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public List<Flashcard> Flashcards { get; set; } // List of flashcards

    public Exam(string name, DateTime date)
    {
        Name = name;
        Date = date;
        Flashcards = new List<Flashcard>(); // Initialize the list of flashcards
    }

    // Method to remove a flashcard
    public bool RemoveFlashcard(string frontText)
    {
        var flashcardToRemove = Flashcards.FirstOrDefault(f => f.FrontText == frontText);
        if (!flashcardToRemove.Equals(default(Flashcard))) // Check if flashcard was found
        {
            Flashcards.Remove(flashcardToRemove);
            return true; // Successfully removed
        }
        return false; // Not found
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
}