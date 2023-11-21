using Xunit;
using Microsoft.EntityFrameworkCore;
using XAM.Models;

namespace MyIntegrationTests
{
    public class XamDbContextTests
    {
        [Fact]
        public void SaveToDatabase_SavesDataCorrectly()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<XamDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase") // Use in-memory database for testing
                .Options;

            var dataHolder = new DataHolder(); // Create a new DataHolder object

            // Act
            using (var context = new XamDbContext(options))
            {
                context.SaveToDatabase(dataHolder); // Save the DataHolder object to the database
            }

            // Assert
            using (var context = new XamDbContext(options))
            {
                var savedDataHolder = context.DataHoldersTable.First();
                Assert.Single(context.DataHoldersTable); // Check that one DataHolder object was saved to the database
                Assert.Equal(dataHolder.CurrentCocktail, savedDataHolder.CurrentCocktail);
                Assert.Equal(dataHolder.Exams, savedDataHolder.Exams);
                Assert.Equal(dataHolder.TimeUntilNextCocktail, savedDataHolder.TimeUntilNextCocktail);
                // Add more assertions for other properties if necessary
            }
        }
    }
}