using Xunit;
using XAM.Controllers;
using XAM.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace MyIntegrationTests
{
    public class ExtensionsTests
    {
        [Fact]
        public void IsMadeOfLettersNumbersAndSpace()
        {
            // Arrange
            var strTrue = "Test 123";
            var strFalse = "@#$%^&*(";

            // Act
            var resultTrue = strTrue.IsMadeOfLettersNumbersAndSpaces();
            var resulFalse = strFalse.IsMadeOfLettersNumbersAndSpaces();

            // Assert
            Assert.True(resultTrue);
            Assert.False(resulFalse);
        }
    }

    public class ExamTests
    {
        [Fact]
        public void Constructor_InitializesPropertiesCorrectly()
        {
            // Arrange
            var name = "Test Exam";
            var date = DateTime.Now;

            // Act
            var exam = new Exam(name, date);

            // Assert
            Assert.Equal(name, exam.Name);
            Assert.Equal(date, exam.Date);
            Assert.Empty(exam.Flashcards);
        }
    }

    public class FlashcardTests
    {
        [Fact]
        public void Constructor_InitializesPropertiesCorrectly()
        {
            // Arrange
            string frontText = "Test Question";
            string backText = "Test Answer";

            // Act
            var flashcard = new Flashcard(frontText, backText);

            // Assert
            Assert.Equal(frontText, flashcard.FrontText);
            Assert.Equal(backText, flashcard.BackText);
        }
    }

    public class ExamIntegrationTests
    {
        [Fact]
        public void AddFlashcard_AddsFlashcardToExam()
        {
            // Arrange
            string examName = "Test Exam";
            DateTime examDate = DateTime.Now;
            var exam = new Exam(examName, examDate);

            string frontText = "Test Question";
            string backText = "Test Answer";
            var flashcard = new Flashcard(frontText, backText);

            // Act
            exam.Flashcards.Add(flashcard);

            // Assert
            Assert.Contains(flashcard, exam.Flashcards);
        }
    }
}