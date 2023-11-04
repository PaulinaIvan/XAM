using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using XAM.Models;

namespace XAM.Controllers;

public class OtherController : Controller
{
    private readonly XamDbContext _context;
    private readonly DataHolder _dataHolder;

    public OtherController(DataHolder dataHolder, XamDbContext context)
    {
        _context = context;
        _dataHolder = dataHolder;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult About() // Temp saver
    {
        _context.DeleteAndReplaceRow(_dataHolder);
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(ErrorViewModel errorModel)
    {
        errorModel.RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        return View(errorModel);
    }
}
