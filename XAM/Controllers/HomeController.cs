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

    public IActionResult UsernameLogin(string username)
    {
        if(username.IsValidExamName())
        {
            _httpContextAccessor.HttpContext?.Session.SetString("CurrentUser", username);
            
            if(_httpContextAccessor.HttpContext?.Session.GetString("CurrentUser") == username)
                return Json(username);
            else
                return StatusCode(501, "Session storage doesn't work.");
        }
        else
        {
            return StatusCode(500, "Invalid username.");
        }
    }

    public IActionResult CheckIfExpired(string username)
    {
        if(_httpContextAccessor.HttpContext?.Session.GetString("CurrentUser") == username)
            return Json(username);
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
