public class Exam
{
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public List<Flashcards> Flashcards { get; set; } // List of flashcards

    public Exam(string name, DateTime date)
    {
        Name = name;
        Date = date;
        Flashcards = new List<Flashcards>(); // Initialize the list of flashcards
    }
}

public struct Flashcards
{
    public string Question { get; set; }
    public string Answer { get; set; }

    public Flashcards(string question, string answer)
    {
        Question = question;
        Answer = answer;
    }
}