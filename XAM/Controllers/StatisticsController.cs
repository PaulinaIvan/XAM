using Microsoft.AspNetCore.Mvc;
using XAM.Models;

namespace XAM.Controllers;

public class StatisticsController : Controller
{
    private readonly DataHolder _dataHolder;

    public StatisticsController(DataHolder dataHolder)
    {
        _dataHolder = dataHolder;
    }

    public IActionResult Statistics()
    {
        return View();
    }

    public IActionResult FetchStatistics()
    {
        var result = new
        {
            lifetimeExams = _dataHolder.LifetimeCreatedExamsCounter,
            lifetimeFlashcards = _dataHolder.LifetimeCreatedFlashcardsCounter,
            challengeHighscoresList = _dataHolder.Exams
                .Where(exam => exam.ChallengeHighscore > 0)
                .Select(exam => new { name = exam.Name, challengeHighscore = exam.ChallengeHighscore })
                .ToArray()
        };
        return Json(result);
    }
}