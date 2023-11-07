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

    public IActionResult SaveToDatabase()
    {
        try
        {
            _context.DeleteAndReplaceRow(_dataHolder);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occured when saving to the database: {ex.Message}");
            return StatusCode(500);
        }

        return Json("Database save successful!");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(ErrorViewModel errorModel)
    {
        errorModel.RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        return View(errorModel);
    }
}
