public class ExamDataHolder
{
    public List<Exam> Exams { get; set; } = new();

    public int LifetimeCreatedExamsCounter { get; set; }
    public int LifetimeCreatedFlashcardsCounter { get; set; }
}