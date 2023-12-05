using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using XAM.Controllers;
using XAM.Models;

namespace MyIntegrationTests
{
    public class RewardControllerTests
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
        public async Task Reward_ReturnsViewResult()
        {
            // Arrange
            var controller = new RewardController(CreateDatabaseContext());
            var mockHttpClient = new Mock<HttpClient>();

            // Act
            var result = await controller.Reward(mockHttpClient.Object);

            // Assert
            Assert.IsType<ViewResult>(result);
        }
    }
}
