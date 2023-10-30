using Microsoft.AspNetCore.Mvc;

namespace XAM.Controllers;

public class TasksController : Controller
{
    private readonly ILogger<TasksController> _logger;
    private readonly DataHolder _dataHolder;

    public record ErrorRecord(string ErrorCode, string ErrorMessage);

    public TasksController(ILogger<TasksController> logger, DataHolder dataHolder)
    {
        _logger = logger;
        _dataHolder = dataHolder;
    }

    ErrorRecord CreateErrorResponse(string ErrorCode, string ErrorMessage = "Unknown error.")
    {
        ErrorRecord ErrorResponse = new(ErrorCode, ErrorMessage);
        return ErrorResponse;
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

        if(flashcards == null || flashcards.Count == 0)
        {
            ErrorRecord errorResponse = CreateErrorResponse("NoFlashcards", $"No flashcards for exam {examName} found.");
            return Json(errorResponse);
        }
        else
        {
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

        if(theExam == null)
        {
            ErrorRecord errorResponse = CreateErrorResponse("NoExamWithName", $"Exam with name {examName} no longer exists.");
            return Json(errorResponse);
        }
        else
        {
            if(theExam.ChallengeHighscore < score)
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
}
