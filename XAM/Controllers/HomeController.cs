using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using XAM.Models;

namespace XAM.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ExamDataSingleton _dataHolder;

    public HomeController(ILogger<HomeController> logger, ExamDataSingleton dataHolder)
    {
        _logger = logger;
        _dataHolder = dataHolder;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Preparation()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult FetchExams()
    {
        return Json(_dataHolder.Exams);
    }

    public IActionResult CreateExam(string name, string date)
    {
        DateTime parsedDate;
        try
        {
            parsedDate = DateTime.Parse(date);
        }
        catch
        {
            Console.WriteLine("Error.");
            return BadRequest("Error.");
        }
        Exam newExam = new(name, parsedDate);

        _dataHolder.Exams.Add(newExam);

        var result = new
        {
            Name = newExam.Name,
            Date = newExam.Date.ToString("yyyy-MM-dd"),
        };
        return Json(result);
    }

    public IActionResult DeleteExam(string examName)
    {
        Exam? examToDelete = _dataHolder.Exams.Find(exam => exam.Name == examName);
        if(examToDelete != null)
        {
            _dataHolder.Exams.Remove(examToDelete);
            return Json("File uploaded and parsed successfully.");
        }
        else
        {
            return BadRequest("Exam not found.");
        }
    }

    public IActionResult CreateFlashcard(string frontText, string backText, string examName)
    {
        try
        {
            _dataHolder.Exams.Find(exam => exam.Name == examName)?.Flashcards.Add(new Flashcard(frontText, backText));
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
            return BadRequest("Error.");
        }
    }

    public IActionResult GetAllExams()
    {
        return Json(_dataHolder.Exams);
    }

    public IActionResult UploadExamFile(IFormFile file)
    {
        if (file != null && file.Length > 0)
        {
            try
            {
                using var reader = new StreamReader(file.OpenReadStream());
                var fileContent = reader.ReadToEnd();
                List<Exam>? examsFromFile = JsonSerializer.Deserialize<List<Exam>>(fileContent,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                        });
                List<Exam> uniqueExams = new();
                if(examsFromFile != null)
                    foreach(Exam examFromFile in examsFromFile)
                    {
                        if(_dataHolder.Exams.Find(exam => exam.Name == examFromFile.Name) == null)
                            uniqueExams.Add(examFromFile);
                    }

                if(examsFromFile != null)
                    _dataHolder.Exams.AddRange(uniqueExams);

                return Json(new { message = "File uploaded and parsed successfully.", list = uniqueExams });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the file.", error = ex.Message });
            }
        }
        else
        {
            return BadRequest(new { message = "No file was selected for upload." });
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
