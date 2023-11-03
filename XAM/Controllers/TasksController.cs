using Microsoft.AspNetCore.Mvc;
using XAM.Models;
using static XAM.Models.HelperClass;

namespace XAM.Controllers;

public class TasksController : Controller
{
    private readonly DataHolder _dataHolder;

    public TasksController(DataHolder dataHolder)
    {
        _dataHolder = dataHolder;
    }

    public IActionResult Tasks()
    {
        return View();
    }

    public IActionResult FetchExamNames()
    {
        List<string> examNames = _dataHolder.Exams.Select(exam => exam.Name).ToList();

        var result = new
        {
            names = examNames
        };
        return Json(result);
    }

    public IActionResult FetchFlashcardsOfExam(string examName)
    {
        List<Flashcard>? flashcards = _dataHolder.Exams.Find(exam => exam.Name == examName)?.Flashcards;

        if (flashcards == null || flashcards.Count == 0)
        {
            ErrorRecord errorResponse = CreateErrorResponse("NoFlashcards", $"No flashcards for exam {examName} found.");
            return Json(errorResponse);
        }
        else
        {
            Shuffle(flashcards);
            var result = new
            {
                flashcards
            };
            return Json(result);
        }
    }

    public IActionResult SetChallengeHighscoreForExam(string examName, int score)
    {
        Exam? theExam = _dataHolder.Exams.Find(exam => exam.Name == examName);

        if (theExam == null)
        {
            ErrorRecord errorResponse = CreateErrorResponse("NoExamWithName", $"Exam with name {examName} no longer exists.");
            return Json(errorResponse);
        }
        else
        {
            if (theExam.ChallengeHighscore < score)
            {
                var result = new
                {
                    text = $@"New {examName} highscore!
                    Old highscore: {theExam.ChallengeHighscore}
                    New highscore: {score}"
                };
                theExam.ChallengeHighscore = score;
                return Json(result);
            }
            else
            {
                var result = new
                {
                    text =
                    $@"No new highscore for {examName}...
                    Score: {score}
                    Highscore: {theExam.ChallengeHighscore}"
                };
                return Json(result);
            }
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
