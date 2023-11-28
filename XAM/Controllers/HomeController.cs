using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using XAM.Models;

namespace XAM.Controllers;

public class HomeController : Controller
{
    private readonly XamDbContext _context;
    private readonly DataHolder _dataHolder;

    public HomeController(DataHolder dataHolder, XamDbContext context)
    {
        _context = context;
        _dataHolder = dataHolder;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Denied()
    {
        return View();
    }

    public IActionResult SaveToDatabaseAction()
    {
        if(_context.SaveToDatabase(_dataHolder))
            return Json("Database save successful!");
        else
            return StatusCode(500);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(ErrorViewModel errorModel)
    {
        errorModel.RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        return View(errorModel);
    }
}
