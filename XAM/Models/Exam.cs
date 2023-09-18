public class Exam
{
    public string Name { get; set; }
    public DateTime Date { get; set; }

    public Exam(string name, DateTime date)
    {
        Name = name;
        Date = date;
    }
}
