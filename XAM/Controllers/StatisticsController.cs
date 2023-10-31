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
            lifetimeFlashcards = _dataHolder.LifetimeCreatedFlashcardsCounter
        };
        return Json(result);
    }
}