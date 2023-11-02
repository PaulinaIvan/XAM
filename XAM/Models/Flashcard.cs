namespace XAM.Models;

public class Flashcard : IComparable<Flashcard>
{
    public string FrontText { get; set; }
    public string BackText { get; set; }

    public Flashcard(string frontText, string backText)
    {
        FrontText = frontText;
        BackText = backText;
    }

    public int CompareTo(Flashcard? other)
    {
        return FrontText.CompareTo(other?.FrontText);
    }
}