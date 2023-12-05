using Xunit;
using XAM.Controllers;
using XAM.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using static XAM.Models.HelperClass;
using static XAM.Controllers.PreparationController;

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

    // ------------------------------------------------------------

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

    // ------------------------------------------------------------

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
            Assert.Equal(1, statisticsHolder.LifetimeCreatedExamsCounter);
            Assert.Equal(1, statisticsHolder.LifetimeCreatedFlashcardsCounter);
            Assert.Equal(1, statisticsHolder.TodayCreatedExamsCounter);
            Assert.Equal(1, statisticsHolder.TodayCreatedFlashcardsCounter);
            Assert.Equal(1, statisticsHolder.TodayHighscoresBeatenCounter);
            Assert.Equal(1, statisticsHolder.TodayChallengesTakenCounter);
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
            Assert.Equal(1, statisticsHolder.LifetimeCreatedExamsCounter);
            Assert.Equal(1, statisticsHolder.LifetimeCreatedFlashcardsCounter);
            Assert.Equal(0, statisticsHolder.TodayCreatedExamsCounter);
            Assert.Equal(0, statisticsHolder.TodayCreatedFlashcardsCounter);
            Assert.Equal(0, statisticsHolder.TodayHighscoresBeatenCounter);
            Assert.Equal(0, statisticsHolder.TodayChallengesTakenCounter);
        }
    }

    // ------------------------------------------------------------

    public class ErrorRecordIntegrationTests
    {
        [Fact]
        public void ErrorRecord_CreatesErrorRecordCorrectly()
        {
            // Arrange
            ErrorRecord errorRecord;

            // Act
            errorRecord = CreateErrorResponse("SomeErrorCode", "Some message.");

            // Assert
            Assert.Equal("SomeErrorCode", errorRecord.ErrorCode);
            Assert.Equal("Some message.", errorRecord.ErrorMessage);
        }

        [Fact]
        public void ErrorRecord_CreatesErrorRecordWithDefaultValue()
        {
            // Arrange
            ErrorRecord errorRecord;

            // Act
            errorRecord = CreateErrorResponse("SomeErrorCode");

            // Assert
            Assert.Equal("SomeErrorCode", errorRecord.ErrorCode);
            Assert.Equal("Unknown error.", errorRecord.ErrorMessage);
        }
    }

    // ------------------------------------------------------------

    public class APIRequestExeptionTests
    {
        [Fact]
        public void APIRequestExeption_ThrowsCorrectly()
        {
            // Arrange
            APIRequestExeption apiRequestExeption = new APIRequestExeption();

            bool catchChecker = false;
            try
            {
                // Act
                throw apiRequestExeption;
            }
            catch (APIRequestExeption)
            {
                catchChecker = true;
            }
            finally
            {
                // Assert
                Assert.True(catchChecker);
            }
        }

        [Fact]
        public void APIRequestExeption_ThrowsWithMessageCorrectly()
        {
            // Arrange
            APIRequestExeption apiRequestExeption = new APIRequestExeption("Some message.");

            try
            {
                // Act
                throw apiRequestExeption;
            }
            catch (APIRequestExeption ex)
            {
                // Assert
                Assert.Equal("Some message.", ex.Message);
            }
        }
    }

    // ------------------------------------------------------------

    public class InvalidExamNameExceptionIntegrationTests
    {
        [Fact]
        public void InvalidExamNameException_ThrowsCorrectly()
        {
            // Arrange
            InvalidExamNameException invalidExamNameException = new InvalidExamNameException();

            bool catchChecker = false;
            try
            {
                // Act
                throw invalidExamNameException;
            }
            catch (InvalidExamNameException)
            {
                catchChecker = true;
            }
            finally
            {
                // Assert
                Assert.True(catchChecker);
            }
        }

        [Fact]
        public void InvalidExamNameException_ThrowsWithMessageCorrectly()
        {
            // Arrange
            InvalidExamNameException invalidExamNameException = new InvalidExamNameException("Some message.");

            try
            {
                // Act
                throw invalidExamNameException;
            }
            catch (InvalidExamNameException ex)
            {
                // Assert
                Assert.Equal("Some message.", ex.Message);
            }
        }
    }
}