using Microsoft.AspNetCore.Mvc;
using XAM.Models;

namespace XAM.Controllers;

public class StatisticsController : Controller
{
    private readonly XamDbContext _context;

    public StatisticsController(XamDbContext context)
    {
        _context = context;
    }

    public IActionResult Statistics()
    {
        return View();
    }

    public IActionResult FetchStatistics()
    {
        DataHolder _dataHolder = _context.GetDataHolder();
        try
        {
            var result = new
            {
                lifetimeExams = _dataHolder.Statistics.LifetimeCreatedExamsCounter,
                lifetimeFlashcards = _dataHolder.Statistics.LifetimeCreatedFlashcardsCounter,
                todayExams = _dataHolder.Statistics.TodayCreatedExamsCounter,
                todayFlashcards = _dataHolder.Statistics.TodayCreatedFlashcardsCounter,
                todayChallengeHighscores = _dataHolder.Statistics.TodayHighscoresBeatenCounter,
                todayChallengeAttempts = _dataHolder.Statistics.TodayChallengesTakenCounter,
                challengeHighscoresList = _dataHolder.Exams
                    .Where(exam => exam.ChallengeHighscore > 0)
                    .Select(exam => new { name = exam.Name, challengeHighscore = exam.ChallengeHighscore })
                    .ToArray()
            };
            return Json(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching statistics: {ex.Message}");
        }
    }
}