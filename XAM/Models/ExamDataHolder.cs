public class ExamDataHolder
{
    public List<Exam> Exams = new();

    public static int LifetimeCreatedExamsCounter { get; set; }
    public static int LifetimeCreatedFlashcardsCounter { get; set; }
}