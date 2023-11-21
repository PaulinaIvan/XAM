using Microsoft.AspNetCore.Mvc;
using XAM.Models;
using static XAM.Models.HelperClass;

namespace XAM.Controllers;

public class TasksController : Controller
{
    private readonly XamDbContext _context;

    public TasksController(XamDbContext context)
    {
        _context = context;
    }

    public IActionResult Tasks()
    {
        return View();
    }

    public IActionResult FetchExamNames()
    {
        DataHolder dataHolder = _context.GetDataHolder();
        List<string> examNames = dataHolder.Exams.Select(exam => exam.Name).ToList();

        return Json(examNames);
    }

    public IActionResult FetchFlashcardsOfExam(string examName)
    {
        DataHolder dataHolder = _context.GetDataHolder();
        List<Flashcard>? flashcards = dataHolder.Exams.Find(exam => exam.Name == examName)?.Flashcards;

        if (flashcards == null || flashcards.Count == 0)
        {
            ErrorRecord errorResponse = CreateErrorResponse("NoFlashcards", $"No flashcards for exam {examName} found.");
            return Json(errorResponse);
        }
        else
        {
            Shuffle(flashcards);
            return Json(flashcards);
        }
    }

    public IActionResult SetChallengeHighscoreForExam(string examName, int score)
    {
        DataHolder dataHolder = _context.GetDataHolder();
        Exam? theExam = dataHolder.Exams.Find(exam => exam.Name == examName);

        if (theExam == null)
        {
            ErrorRecord errorResponse = CreateErrorResponse("NoExamWithName", $"Exam with name {examName} no longer exists.");
            return Json(errorResponse);
        }
        else
        {
            dataHolder.Statistics.TodayChallengesTakenCounter++;
            string result;
            if (theExam.ChallengeHighscore < score)
            {
                dataHolder.Statistics.TodayHighscoresBeatenCounter++;
                int oldHighscore = theExam.ChallengeHighscore;
                theExam.ChallengeHighscore = score;
                result = $@"New {examName} highscore!
                                Old highscore: {oldHighscore}
                                New highscore: {score}";
            }
            else
            {
                result = $@"No new highscore for {examName}...
                                Score: {score}
                                Highscore: {theExam.ChallengeHighscore}";
                
            }
            _context.SaveToDatabase(dataHolder);
            return Json(result);
        }
    }

    static void Shuffle<T>(List<T> list) where T : class, IComparable<T> // 2. Create generic method, event or delegate; define at least 2 generic constraints
    {
        Random random = new();

        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = random.Next(0, i + 1);

            (list[randomIndex], list[i]) = (list[i], list[randomIndex]);
        }
    }
}
