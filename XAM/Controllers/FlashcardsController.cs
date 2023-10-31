using Microsoft.AspNetCore.Mvc;
using XAM.Models;

namespace XAM.Controllers;

public class FlashcardsController : Controller
{
    private readonly DataHolder _dataHolder;

    public FlashcardsController(DataHolder dataHolder)
    {
        _dataHolder = dataHolder;
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

}