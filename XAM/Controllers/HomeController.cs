using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using XAM.Models;

namespace XAM.Controllers;

public class HomeController : Controller
{
    // Requirements not achieved:
    // 1. Relational database is used for storing data
    // 2. Create generic method, event or delegate; define at least 2 generic constraints
    // 3. Delegates usage
    // 4. Create at least 1 exception type and throw it; meaningfully deal with it; (most of the exceptions are logged to a file or a server)
    // 5. Lambda expressions usage
    // 6. Usage of threading via Thread class
    // 7. Usage of async/await
    // 8. Use at least 1 concurrent collection or Monitor
    // 9. Regex usage
    // 10. No instances are created using 'new' keyword, dependency injection is used everywhere
    // 11. Unit and integration tests coverage at least 20%

    private readonly ILogger<HomeController> _logger;
    private readonly DataHolder _dataHolder;

    public record ErrorRecord(string ErrorCode, string ErrorMessage);

    public HomeController(ILogger<HomeController> logger, DataHolder dataHolder)
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

    public IActionResult Statistics()
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

    public IActionResult FetchStatistics()
    {
        var result = new
        {
            lifetimeExams = _dataHolder.LifetimeCreatedExamsCounter,
            lifetimeFlashcards = _dataHolder.LifetimeCreatedFlashcardsCounter,
            challengeHighscoresList = _dataHolder.Exams
                .Where(exam => exam.ChallengeHighscore > 0)
                .Select(exam => new { name = exam.Name, challengeHighscore = exam.ChallengeHighscore })
                .ToArray()

        };
        return Json(result);
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
        ++_dataHolder.LifetimeCreatedExamsCounter;

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

            ++_dataHolder.LifetimeCreatedFlashcardsCounter;

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

    [HttpGet]
    public IActionResult DownloadAllData()
    {
        var jsonContent = JsonSerializer.Serialize(_dataHolder);

        Response.Headers.Add("Content-Disposition", "attachment");
        return Content(jsonContent, "application/json");
    }

    public IActionResult UploadDataFile(IFormFile file)
    {
        if (file != null && file.Length > 0)
        {
            try
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    var fileContent = reader.ReadToEnd();
                    DataHolder? newDataHolder = JsonSerializer.Deserialize<DataHolder>(fileContent,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                        });

                    if (newDataHolder != null)
                    {
                        List<Exam> examsNotOnFrontend = newDataHolder.Exams.Where(examA => !_dataHolder.Exams.Any(examB => examA.Name == examB.Name)).ToList();
                        _dataHolder.Exams.AddRange(examsNotOnFrontend);
                        _dataHolder.LifetimeCreatedExamsCounter = newDataHolder.LifetimeCreatedExamsCounter;
                        _dataHolder.LifetimeCreatedFlashcardsCounter = newDataHolder.LifetimeCreatedFlashcardsCounter;

                        return Json(new { message = "File uploaded and parsed successfully.", list = examsNotOnFrontend });
                    }
                }

                return StatusCode(500, new { message = "An error occurred while processing the file." });
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
