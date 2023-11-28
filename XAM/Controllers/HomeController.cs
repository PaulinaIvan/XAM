using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using XAM.Models;
using static XAM.Models.HelperClass;

namespace XAM.Controllers;

public class HomeController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HomeController(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
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

    public IActionResult Denied()
    {
        return View();
    }

    public IActionResult UsernameLogin(string username)
    {
        if(_context.SaveToDatabase(_dataHolder))
            return Json("Database save successful!");
        else
        {
            ErrorRecord errorResponse = CreateErrorResponse("NoSession", "No current user session exists.");
            return Json(errorResponse);
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(ErrorViewModel errorModel)
    {
        errorModel.RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        return View(errorModel);
    }
}
