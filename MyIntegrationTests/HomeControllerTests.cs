using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using XAM.Controllers;

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
    }
}
