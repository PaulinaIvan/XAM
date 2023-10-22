using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using XAM.Models;

namespace XAM.Controllers;

public class HomeController : Controller
{
    // Requirements not achieved:

    private readonly ILogger<HomeController> _logger;
    private readonly ExamDataSingleton _dataHolder;

    public record ErrorRecord(string ErrorCode, string ErrorMessage);

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
        List<Exam> correctlyNamedExams = _dataHolder.Exams.Where(exam => exam.Name.IsMadeOfLettersNumbersAndSpaces()).ToList();

        // (Same thing as above, but with queries)
        // List<Exam> correctlyNamedExams = (from exam in _dataHolder.Exams where exam.Name.IsMadeOfLettersNumbersAndSpaces() select exam).ToList();

        return Json(correctlyNamedExams);
    }

    public IActionResult CreateExam(string name, string date)
    {
        if (!name.IsMadeOfLettersNumbersAndSpaces())
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
        Exam newExam = new(date: parsedDate, name: name);

        _dataHolder.Exams.Add(newExam);

        var result = new
        {
            Name = newExam.Name,
            Date = newExam.Date.ToString("yyyy-MM-dd"),
        };
        return Json(result);
    }

    ErrorRecord CreateErrorResponse(string ErrorCode, string ErrorMessage = "Unknown error.")
    {
        ErrorRecord ErrorResponse = new(ErrorCode, ErrorMessage);
        return ErrorResponse;
    }

    public IActionResult DeleteExam(string examName)
    {
        Exam? examToDelete = _dataHolder.Exams.Find(exam => exam.Name == examName);
        if (examToDelete != null)
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
            Exam? exam = _dataHolder.Exams.Find(exam => exam.Name == examName);
            if (exam == null)
            {
                return BadRequest("Exam not found.");
            }

            Flashcard flashcard = new Flashcard(frontText, backText);
            exam.Flashcards.Add(flashcard);

            int index = exam.Flashcards.IndexOf(flashcard);

            var result = new
            {
                FrontText = frontText,
                BackText = backText,
                ExamName = examName,
                Index = index
            };
            return Json(result);
        }
        catch
        {
            Console.WriteLine("Error.");
            return BadRequest("Error.");
        }
    }

    [HttpDelete]
    public IActionResult DeleteFlashcard(string examName, int flashcardIndex)
    {
        try
        {
            Exam? exam = _dataHolder.Exams.Find(exam => exam.Name == examName);
            if (exam == null)
            {
                return BadRequest("Exam not found.");
            }

            if (flashcardIndex < 0 || flashcardIndex >= exam.Flashcards.Count)
            {
                return BadRequest("Flashcard not found.");
            }

            exam.Flashcards.RemoveAt(flashcardIndex);

            return Ok();
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
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    var fileContent = reader.ReadToEnd();
                    List<Exam>? examsFromFile = JsonSerializer.Deserialize<List<Exam>>(fileContent,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                        });

                    if (examsFromFile != null)
                        foreach (Exam examFromFile in examsFromFile)
                        {
                            if (_dataHolder.Exams.Find(exam => exam.Name == examFromFile.Name) == null)
                                uniqueExams.Add(examFromFile);
                        }

                    if (examsFromFile != null)
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
