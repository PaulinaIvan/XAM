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
        public void IsValidExamName()
        {
            // Arrange
            var strTrue = "Test 123";
            var strFalse = "@#$%^&*(";

            // Act
            var resultTrue = strTrue.IsValidExamName();
            var resulFalse = strFalse.IsValidExamName();

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

    public class StatisticsIntegrationTests
    {
        [Fact]
        public void StatisticsHolder_AddsStatisticsCorrectly()
        {
            // Arrange
            StatisticsHolder statisticsHolder = new();

            // Act
            statisticsHolder.LifetimeCreatedExamsCounter++;
            statisticsHolder.LifetimeCreatedFlashcardsCounter++;
            statisticsHolder.TodayCreatedExamsCounter++;
            statisticsHolder.TodayCreatedFlashcardsCounter++;
            statisticsHolder.TodayHighscoresBeatenCounter++;
            statisticsHolder.TodayChallengesTakenCounter++;

            // Assert
            Assert.Equal(statisticsHolder.LifetimeCreatedExamsCounter, 1);
            Assert.Equal(statisticsHolder.LifetimeCreatedFlashcardsCounter, 1);
            Assert.Equal(statisticsHolder.TodayCreatedExamsCounter, 1);
            Assert.Equal(statisticsHolder.TodayCreatedFlashcardsCounter, 1);
            Assert.Equal(statisticsHolder.TodayHighscoresBeatenCounter, 1);
            Assert.Equal(statisticsHolder.TodayChallengesTakenCounter, 1);
        }

        [Fact]
        public void StatisticsHolder_IsEligibleForCocktail()
        {
            // Arrange
            StatisticsHolder statisticsHolder1 = new();
            StatisticsHolder statisticsHolder2 = new();

            // Act
            statisticsHolder1.TodayHighscoresBeatenCounter = 1;
            statisticsHolder1.TodayChallengesTakenCounter = 1;

            statisticsHolder2.TodayHighscoresBeatenCounter = 2;
            statisticsHolder2.TodayChallengesTakenCounter = 2;

            // Assert
            Assert.False(statisticsHolder1.IsEligibleForCocktail());
            Assert.True(statisticsHolder2.IsEligibleForCocktail());
        }

        [Fact]
        public void StatisticsHolder_ResetTodaysStatistics()
        {
            // Arrange
            StatisticsHolder statisticsHolder = new();

            // Act
            statisticsHolder.LifetimeCreatedExamsCounter = 1;
            statisticsHolder.LifetimeCreatedFlashcardsCounter = 1;
            statisticsHolder.TodayCreatedExamsCounter = 1;
            statisticsHolder.TodayCreatedFlashcardsCounter = 1;
            statisticsHolder.TodayHighscoresBeatenCounter = 1;
            statisticsHolder.TodayChallengesTakenCounter = 1;

            statisticsHolder.ResetTodaysStatistics();

            // Assert
            Assert.Equal(statisticsHolder.LifetimeCreatedExamsCounter, 1);
            Assert.Equal(statisticsHolder.LifetimeCreatedFlashcardsCounter, 1);
            Assert.Equal(statisticsHolder.TodayCreatedExamsCounter, 0);
            Assert.Equal(statisticsHolder.TodayCreatedFlashcardsCounter, 0);
            Assert.Equal(statisticsHolder.TodayHighscoresBeatenCounter, 0);
            Assert.Equal(statisticsHolder.TodayChallengesTakenCounter, 0);
        }
    }
}