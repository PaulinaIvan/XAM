using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using XAM.Models;

namespace XAM.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private List<Exam> Exams = new();

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Preparation()
    {
        //Exam class testing, try it out
        DateTime DT = new DateTime(2015, 12, 20);
        Exam bio = new Exam("BIOLOGIJA", DT);
        Console.WriteLine(bio.Name);

        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Test()
    {
        return View();
    }

    public IActionResult CreateExam(string name, string date)
    {
        try
        {
            Exam newExam = new(name, DateTime.Parse(date));
            Exams.Add(newExam);

            var result = new
            {
                Name = name,
                Date = date,
            };
            return Json(result);
        }
        catch
        {
            Console.WriteLine("Error.");
            return Json(null);
        }
    }

    public IActionResult CreateFlashcard(string frontText, string backText, string examName)
    {
        try
        {
            Exams.Find(exam => exam.Name == examName)?.Flashcards.Add(new Flashcard(frontText, backText));
            var result = new
            {
                FrontText = frontText,
                BackText = backText,
                ExamName = examName
            };
            return Json(result);
        }
        catch
        {
            Console.WriteLine("Error.");
            return Json(null);
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
