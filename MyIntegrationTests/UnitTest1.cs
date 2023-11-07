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
            Assert.Equal(errorRecord.ErrorCode, "SomeErrorCode");
            Assert.Equal(errorRecord.ErrorMessage, "Some message.");
        }

        [Fact]
        public void ErrorRecord_CreatesErrorRecordWithDefaultValue()
        {
            // Arrange
            ErrorRecord errorRecord;

            // Act
            errorRecord = CreateErrorResponse("SomeErrorCode");

            // Assert
            Assert.Equal(errorRecord.ErrorCode, "SomeErrorCode");
            Assert.Equal(errorRecord.ErrorMessage, "Unknown error.");
        }
    }

    public class APIRequestExeptionIntegrationTests
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
            catch (APIRequestExeption ex)
            {
                catchChecker = true;
            }
            finally
            {
                // Assert
                Assert.True(catchChecker);
            }
        }

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
                Assert.Equal(ex.Message, "Some message.");
            }
        }
    }

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
            catch (InvalidExamNameException ex)
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
                Assert.Equal(ex.Message, "Some message.");
            }
        }
    }

    public class DataHolderIntegrationTests
    {
        [Fact]
        public void DataHolder_HoldsSimplePropertiesCorrectly()
        {
            // Arrange
            DataHolder dataHolder = new DataHolder();

            // Act
            dataHolder.DataHolderId = 5;

            List<Exam> exams = new();
            dataHolder.Exams = exams;

            StatisticsHolder statisticsHolder = new();
            dataHolder.Statistics = statisticsHolder;

            dataHolder.CurrentCocktail = "Some cocktail.";

            // Assert
            Assert.Equal(dataHolder.DataHolderId, 5);
            Assert.Equal(dataHolder.Exams, exams);
            Assert.Equal(dataHolder.Statistics, statisticsHolder);
            Assert.Equal(dataHolder.CurrentCocktail, "Some cocktail.");
        }

        [Fact]
        public void DataHolder_HiddenUtcWorksCorrectly()
        {
            // Arrange
            DataHolder dataHolder = new DataHolder();
            DateTime dateTime = new DateTime(2023,11,01, 00,00,00);

            // Act
            dataHolder.TimeUntilNextCocktail = dateTime;

            // Assert
            Assert.Equal(dataHolder.TimeUntilNextCocktail, dateTime);
        }
    }

    public class GetExamsNotOldDataHolderUnitTests
    {
        [Fact]
        public void GetExamsNotOldDataHolder_ReturnsCorrectly()
        {
            // Arrange
            DataHolder dataHolder1 = new DataHolder();
            DataHolder dataHolder2 = new DataHolder();
            List<Exam> exams = new List<Exam>
            {
                new Exam("Math", new DateTime(2023,12,01, 00,00,00)),
                new Exam("Science", new DateTime(2023,12,05, 00,00,00)),
                new Exam("History", new DateTime(2023,12,07, 00,00,00))
            };
            List<Exam> examsNotOnOld = new();

            // Act
            dataHolder1.Exams.Add(exams[0]);
            dataHolder1.Exams.Add(exams[1]);
            dataHolder2.Exams = exams;

            examsNotOnOld = GetExamsNotOldDataHolder(dataHolder1, dataHolder2);

            // Assert
            Assert.Equal(examsNotOnOld[0], exams[2]);
        }
    }
}