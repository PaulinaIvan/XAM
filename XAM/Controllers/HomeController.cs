using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using XAM.Models;

namespace XAM.Controllers;

public class HomeController : Controller
{
    // Requirements not achieved:
    // 1.4 Creating and using your own enum;
    // 7. Create and use at least 1 generic type;
    // 8. Boxing and Unboxing;

    private readonly ILogger<HomeController> _logger;
    private readonly ExamDataSingleton _dataHolder;

    public record ErrorRecord(string ErrorCode, string ErrorMessage); // 1.3 Creating and using your own record;

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
        // 9.1. LINQ to Objects usage (methods);
        List<Exam> correctlyNamedExams = _dataHolder.Exams.Where(exam => exam.Name.IsMadeOfLettersNumbersAndSpaces()).ToList();

        // 9.2. LINQ to Objects usage (queries);
        // (Same thing as above, but with queries)
        // List<Exam> correctlyNamedExams = (from exam in _dataHolder.Exams where exam.Name.IsMadeOfLettersNumbersAndSpaces() select exam).ToList();

        return Json(correctlyNamedExams);
    }

    public IActionResult CreateExam(string name, string date)
    {
        if(!name.IsMadeOfLettersNumbersAndSpaces()) // 4. Extension method usage.
        {
            string error = "Invalid exam name.";
            Console.WriteLine(error);

            ErrorRecord errorResponse = CreateErrorResponse("BadName", error);
            return Json(errorResponse);
        }

        DateTime parsedDate;
        try
        {
            parsedDate = DateTime.Parse(date);
        }
        catch
        {
            string error = "Unparseable date.";
            Console.WriteLine(error);

            ErrorRecord errorResponse = CreateErrorResponse("BadDate", error);
            return Json(errorResponse);
        }
        Exam newExam = new(date: parsedDate, name: name); // 3.1. Named argument usage;

        _dataHolder.Exams.Add(newExam);

        var result = new
        {
            Name = newExam.Name,
            Date = newExam.Date.ToString("yyyy-MM-dd"),
        };
        return Json(result);
    }

    ErrorRecord CreateErrorResponse(string ErrorCode, string ErrorMessage = "Unknown error.") // 3.2. Optional argument usage;
    {
        ErrorRecord ErrorResponse = new(ErrorCode, ErrorMessage);
        return ErrorResponse;
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
                List<Exam> uniqueExams = new();
                using(var reader = new StreamReader(file.OpenReadStream())) // 6. Reading from a file using a stream.
                {
                    var fileContent = reader.ReadToEnd();
                    List<Exam>? examsFromFile = JsonSerializer.Deserialize<List<Exam>>(fileContent,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                        });
                    
                    if(examsFromFile != null)
                        foreach(Exam examFromFile in examsFromFile) // 5. Iterating through collection the right way.
                        {
                            if(_dataHolder.Exams.Find(exam => exam.Name == examFromFile.Name) == null)
                                uniqueExams.Add(examFromFile);
                        }

                    if(examsFromFile != null)
                        _dataHolder.Exams.AddRange(uniqueExams);
                }
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
