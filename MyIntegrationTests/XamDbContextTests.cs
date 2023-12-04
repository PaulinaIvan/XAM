using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using Xunit;
using XAM.Models;

namespace MyIntegrationTests
{
    public class XamDbContextTests
    {
        private readonly DbContextOptions<XamDbContext> _options;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;

        public XamDbContextTests()
        {
            _options = new DbContextOptionsBuilder<XamDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase") // Use in-memory database for testing
                .Options;

            var mockHttpContext = new Mock<HttpContext>();
            var mockClaimsPrincipal = new Mock<ClaimsPrincipal>();
            var mockSession = new Mock<ISession>();

            mockClaimsPrincipal.Setup(x => x.Identity.Name).Returns("testUser");
            mockHttpContext.Setup(x => x.User).Returns(mockClaimsPrincipal.Object);
            mockHttpContext.Setup(x => x.Session).Returns(mockSession.Object);

            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext.Object);
        }

        [Fact]
        public void GetDataHolder_ReturnsDataHolder()
        {
            // Arrange
            var dataHolder = new DataHolder { OwnerUsername = "testUser" };

            using (var context = new XamDbContext(_options, _mockHttpContextAccessor.Object))
            {
                context.DataHoldersTable.Add(dataHolder);
                context.SaveChanges();
            }

            // Act
            DataHolder result;
            using (var context = new XamDbContext(_options, _mockHttpContextAccessor.Object))
            {
                result = context.DataHoldersTable.First();
            }

            // Assert
            Assert.Equal("testUser", result.OwnerUsername);
        }

        [Fact]
        public void SaveToDatabase_UpdatesExistingDataHolder()
        {
            // Arrange
            var dataHolder = new DataHolder { OwnerUsername = "testUser" };

            using (var context = new XamDbContext(_options, _mockHttpContextAccessor.Object))
            {
                context.DataHoldersTable.Add(dataHolder);
                context.SaveChanges();
            }

            // Act
            using (var context = new XamDbContext(_options, _mockHttpContextAccessor.Object))
            {
                var dataHolderToUpdate = context.DataHoldersTable.First();
                dataHolderToUpdate.OwnerUsername = "updatedUser";
                context.SaveToDatabase(dataHolderToUpdate);
            }

            // Assert
            using (var context = new XamDbContext(_options, _mockHttpContextAccessor.Object))
            {
                var savedDataHolder = context.DataHoldersTable.First();
                Assert.Equal("updatedUser", savedDataHolder.OwnerUsername);
            }
        }
    }
}