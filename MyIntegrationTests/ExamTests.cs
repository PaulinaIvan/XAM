using Xunit;
using XAM.Models;
using System;
using System.Collections.Generic;

namespace MyIntegrationTests
{
    public class ExamTests
    {
        [Fact]
        public void Exam_Constructor_SetsPropertiesCorrectly()
        {
            // Arrange
            var name = "Test Exam";
            var date = DateTime.UtcNow;

            // Act
            var exam = new Exam(name, date);

            // Assert
            Assert.Equal(name, exam.Name);
            Assert.Equal(date, exam.Date);
            Assert.Empty(exam.Flashcards);
        }

        [Fact]
        public void Exam_CompareTo_ReturnsCorrectOrder()
        {
            // Arrange
            var exam1 = new Exam("A", DateTime.UtcNow);
            var exam2 = new Exam("B", DateTime.UtcNow);

            // Act
            var result = exam1.CompareTo(exam2);

            // Assert
            Assert.True(result < 0);
        }

        [Fact]
        public void Exam_DeleteFlashcard_RemovesFlashcardFromList()
        {
            // Arrange
            var exam = new Exam("Test", DateTime.UtcNow);
            var flashcard1 = new Flashcard("Front Text", "Back Text");
            var flashcard2 = new Flashcard("Question", "Answer");
            exam.Flashcards.Add(flashcard1);
            exam.Flashcards.Add(flashcard2);

            // Act
            exam.DeleteFlashcard(flashcard1);

            // Assert
            Assert.DoesNotContain(flashcard1, exam.Flashcards);
        }
    }
}