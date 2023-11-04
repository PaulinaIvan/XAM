using Microsoft.AspNetCore.Mvc;
using XAM.Models;

namespace XAM.Controllers;

public class RewardController : Controller
{
    private readonly DataHolder _dataHolder;

    public RewardController(DataHolder dataHolder)
    {
        _dataHolder = dataHolder;
    }

    public IActionResult Reward()
    {
        return View();
    }

    public bool isEligibleForReward()
    {
        return _dataHolder.TodaysAchievements.ChallengesTaken >= 2 && _dataHolder.TodaysAchievements.HighscoresBeaten >= 1;
    }

    [HttpGet]
    public IActionResult FetchCocktail()
    {
        if (string.IsNullOrEmpty(_dataHolder.TodaysCocktail))
        {
            return NotFound("Todays cocktail is not available.");
        }

        return Content(_dataHolder.TodaysCocktail, "application/json");
    }

}