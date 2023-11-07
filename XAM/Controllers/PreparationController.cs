using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using XAM.Models;
using static XAM.Models.HelperClass;

namespace XAM.Controllers;

public class PreparationController : Controller
{
    private readonly DataHolder _dataHolder;

    public PreparationController(DataHolder dataHolder)
    {
        _dataHolder = dataHolder;
    }

    public IActionResult Preparation()
    {
        return View();
    }

    delegate bool StringChecker(string s); // 3. Delegates usage
    readonly StringChecker checkForLetterNumbersAndSpaces = s => s.IsValidExamName(); // 5. Lambda expressions usage
    public IActionResult FetchExams()
    {
        List<Exam> correctlyNamedExams = _dataHolder.Exams.Where(exam => checkForLetterNumbersAndSpaces(exam.Name)).ToList();

        // (Same thing as above, but with queries)
        // List<Exam> correctlyNamedExams = (from exam in _dataHolder.Exams where exam.Name.IsMadeOfLettersNumbersAndSpaces() select exam).ToList();

        return Json(correctlyNamedExams);
    }

    public IActionResult CreateExam(string name, string date)
    {
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

        _dataHolder.Exams.Add(newExam);
        ++_dataHolder.Statistics.LifetimeCreatedExamsCounter;
        ++_dataHolder.Statistics.TodayCreatedExamsCounter;

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

            Flashcard flashcard = new(frontText, backText);
            exam.Flashcards.Add(flashcard);

            ++_dataHolder.Statistics.LifetimeCreatedFlashcardsCounter;
            ++_dataHolder.Statistics.TodayCreatedFlashcardsCounter;

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
                    DataHolder? newDataHolder = JsonSerializer.Deserialize<DataHolder>(fileContent);

                    if (newDataHolder != null)
                    {
                        List<Exam> examsNotOnFrontend = GetExamsNotOldDataHolder(_dataHolder, newDataHolder);
                        _dataHolder.Exams.AddRange(examsNotOnFrontend);
                        _dataHolder.Statistics.LifetimeCreatedExamsCounter = newDataHolder.Statistics.LifetimeCreatedExamsCounter;
                        _dataHolder.Statistics.LifetimeCreatedFlashcardsCounter = newDataHolder.Statistics.LifetimeCreatedFlashcardsCounter;

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
