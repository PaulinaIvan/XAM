public class Exam
{
    public string Name { get; set; }
    public DateTime Date { get; set; }

    //Each exam will also have flashcards data, but for now name and date should be alright

    public Exam(string name, DateTime date)
    {
        Name = name;
        Date = date;
    }
}
