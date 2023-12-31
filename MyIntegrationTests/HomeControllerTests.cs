using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using XAM.Controllers;
using XAM.Models;
using static XAM.Models.HelperClass;

namespace MyIntegrationTests
{
    public class HomeControllerTests
    {
        [Fact]
        public void Index_ReturnsViewResult()
        {
            // Arrange
            var controller = new HomeController(Mock.Of<IHttpContextAccessor>());

            // Act
            var result = controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void About_ReturnsViewResult()
        {
            // Arrange
            var controller = new HomeController(Mock.Of<IHttpContextAccessor>());

            // Act
            var result = controller.About();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Denied_ReturnsViewResult()
        {
            // Arrange
            var controller = new HomeController(Mock.Of<IHttpContextAccessor>());

            // Act
            var result = controller.Denied();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void UsernameLogin_InvalidUsername_ReturnsStatusCode500()
        {
            // Arrange
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var controller = new HomeController(httpContextAccessorMock.Object);
            const string invalidUsername = "invalid_name";

            // Act
            var result = controller.UsernameLogin(invalidUsername) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            Assert.Equal("Invalid username.", result.Value);
        }

        [Fact]
        public void CheckIfExpired_NoUserSession_ReturnsJsonError()
        {
            // Arrange
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var httpContextMock = new Mock<HttpContext>();
            var sessionMock = new Mock<ISession>();

            httpContextAccessorMock.SetupGet(x => x.HttpContext).Returns(httpContextMock.Object);
            httpContextMock.SetupGet(x => x.Session).Returns(sessionMock.Object);

            var controller = new HomeController(httpContextAccessorMock.Object);
            const string username = "testUser";

            // Act
            var result = controller.CheckIfExpired("differentUser") as JsonResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ErrorRecord>(result.Value);
        }
    }
}
