using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using XAM.Models;
using static XAM.Models.HelperClass;

namespace XAM.Controllers;

public class PreparationController : Controller
{
    private readonly XamDbContext _context;

    public PreparationController(XamDbContext context)
    {
        _context = context;
    }

    public IActionResult Preparation()
    {
        return View();
    }

    delegate bool StringChecker(string s);
    readonly StringChecker checkForLetterNumbersAndSpaces = s => s.IsValidExamName();
    public IActionResult FetchExams()
    {
        DataHolder dataHolder = _context.GetDataHolder();
        List<Exam> correctlyNamedExams = dataHolder.Exams.Where(exam => checkForLetterNumbersAndSpaces(exam.Name)).ToList();

        // (Same thing as above, but with queries)
        // List<Exam> correctlyNamedExams = (from exam in _dataHolder.Exams where exam.Name.IsMadeOfLettersNumbersAndSpaces() select exam).ToList();

        return Json(correctlyNamedExams);
    }

    public IActionResult CreateExam(string name, string date)
    {
        DataHolder dataHolder = _context.GetDataHolder();
        if (!name.IsValidExamName())
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

        dataHolder.Exams.Add(newExam);
        ++dataHolder.Statistics.LifetimeCreatedExamsCounter;
        ++dataHolder.Statistics.TodayCreatedExamsCounter;

        var result = new
        {
            Name = newExam.Name,
            Date = newExam.Date.ToString("yyyy-MM-dd"),
        };
        _context.SaveToDatabase(dataHolder);
        return Json(result);
    }

    public IActionResult DeleteExam(string examName)
    {
        DataHolder dataHolder = _context.GetDataHolder();
        Exam? examToDelete = dataHolder.Exams.Find(exam => exam.Name == examName);
        if (examToDelete != null)
        {
            dataHolder.Exams.Remove(examToDelete);
            _context.SaveToDatabase(dataHolder);
            return Json("File uploaded and parsed successfully.");
        }
        else
        {
            return BadRequest("Exam not found.");
        }
    }

    public IActionResult CreateFlashcard(string frontText, string backText, string examName)
    {
        DataHolder dataHolder = _context.GetDataHolder();
        try
        {
            Exam? exam = dataHolder.Exams.Find(exam => exam.Name == examName);
            if (exam == null)
            {
                return BadRequest("Exam not found.");
            }

            Flashcard flashcard = new(frontText, backText);
            exam.Flashcards.Add(flashcard);

            ++dataHolder.Statistics.LifetimeCreatedFlashcardsCounter;
            ++dataHolder.Statistics.TodayCreatedFlashcardsCounter;

            int index = exam.Flashcards.IndexOf(flashcard);

            var result = new
            {
                FrontText = frontText,
                BackText = backText,
                ExamName = examName,
                Index = index
            };
            _context.SaveToDatabase(dataHolder);
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
        DataHolder dataHolder = _context.GetDataHolder();
        try
        {
            Exam? exam = dataHolder.Exams.Find(exam => exam.Name == examName);
            if (exam == null)
            {
                return BadRequest("Exam not found.");
            }

            if (flashcardIndex < 0 || flashcardIndex >= exam.Flashcards.Count)
            {
                return BadRequest("Flashcard not found.");
            }

            exam.Flashcards.RemoveAt(flashcardIndex);

            _context.SaveToDatabase(dataHolder);
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
        DataHolder dataHolder = _context.GetDataHolder();
        var jsonContent = JsonSerializer.Serialize(dataHolder);

        Response.Headers.Add("Content-Disposition", "attachment");
        return Content(jsonContent, "application/json");
    }

    public IActionResult UploadDataFile(IFormFile file)
    {
        DataHolder dataHolder = _context.GetDataHolder();
        if (file != null && file.Length > 0)
        {
            try
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    var fileContent = reader.ReadToEnd();
                    DataHolder? newDataHolder = JsonSerializer.Deserialize<DataHolder>(fileContent);

                    if (newDataHolder != null)
                    {
                        List<Exam> examsNotOnFrontend = GetExamsNotOldDataHolder(dataHolder, newDataHolder);
                        dataHolder.Exams.AddRange(examsNotOnFrontend);
                        dataHolder.Statistics.LifetimeCreatedExamsCounter = newDataHolder.Statistics.LifetimeCreatedExamsCounter;
                        dataHolder.Statistics.LifetimeCreatedFlashcardsCounter = newDataHolder.Statistics.LifetimeCreatedFlashcardsCounter;

                        _context.SaveToDatabase(dataHolder);
                        return Json(examsNotOnFrontend);
                    }
                }

                return StatusCode(500, "An error occurred while processing the file.");
            }
            catch (Exception ex)
            {
                return StatusCode(501, $"An error occurred while processing the file: {ex.Message}");
            }
        }
        else
        {
            return BadRequest("No file was selected for upload.");
        }
    }

    public static List<Exam> GetExamsNotOldDataHolder(DataHolder oldDataHolder, DataHolder newDataHolder)
    {
        return newDataHolder.Exams.Where(examA => !oldDataHolder.Exams.Any(examB => examA.Name == examB.Name)).ToList();
    }
}
