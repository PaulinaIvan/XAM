using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using XAM.Controllers;
using XAM.Models;

namespace MyIntegrationTests
{
    public class StatisticsControllerTests
    {
        private XamDbContext CreateDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<XamDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new XamDbContext(options, Mock.Of<IHttpContextAccessor>());
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public void Statistics_ReturnsViewResult()
        {
            // Arrange
            var controller = new StatisticsController(CreateDatabaseContext());

            // Act
            var result = controller.Statistics();

            // Assert
            Assert.IsType<ViewResult>(result);
        }
    }
}
