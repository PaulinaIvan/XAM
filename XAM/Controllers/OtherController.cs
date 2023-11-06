using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using XAM.Models;

namespace XAM.Controllers;

public class OtherController : Controller
{
    private readonly XamDbContext _context;
    private readonly DataHolder _dataHolder;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public OtherController(DataHolder dataHolder, XamDbContext context, IHostApplicationLifetime applicationLifetime)
    {
        _context = context;
        _dataHolder = dataHolder;
        _applicationLifetime = applicationLifetime;

        _applicationLifetime.ApplicationStopping.Register(() =>
        {
            DateTime lastShutdownDate = DateTime.Now.ToUniversalTime();
            Console.WriteLine(lastShutdownDate);
        });
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
        if(_context.DeleteAndReplaceRow(_dataHolder))
            return Json(true);
        else
            return BadRequest(false);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(ErrorViewModel errorModel)
    {
        errorModel.RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        return View(errorModel);
    }
}
