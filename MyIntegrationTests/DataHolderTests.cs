using Xunit;
using XAM.Models;
using System;

namespace MyIntegrationTests
{
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
            Assert.Equal(5, dataHolder.DataHolderId);
            Assert.Equal(dataHolder.Exams, exams);
            Assert.Equal(dataHolder.Statistics, statisticsHolder);
            Assert.Equal("Some cocktail.", dataHolder.CurrentCocktail);
        }

        [Fact]
        public void DataHolder_HiddenUtcWorksCorrectly()
        {
            // Arrange
            DataHolder dataHolder = new DataHolder();
            DateTime dateTime = new DateTime(2023, 11, 01, 00, 00, 00);

            // Act
            dataHolder.TimeUntilNextCocktail = dateTime;

            // Assert
            Assert.Equal(dataHolder.TimeUntilNextCocktail, dateTime);
        }

        [Fact]
        public void DataHolder_ReturnsNull_WhenTimeUntilNextCocktailHasNoValue()
        {
            // Arrange
            var dataHolder = new DataHolder();

            // Act
            var result = dataHolder.TimeUntilNextCocktail;

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void DataHolder_SetsTimeUntilNextCocktailToNull_WhenValueHasNoValue()
        {
            // Arrange
            var dataHolder = new DataHolder();

            // Act
            dataHolder.TimeUntilNextCocktail = null;

            // Assert
            Assert.Null(dataHolder.TimeUntilNextCocktail);
        }
    }
}