using Xunit;
using XAM.Models;

namespace MyIntegrationTests
{
    public class ErrorViewModelTests
    {
        [Fact]
        public void RequestId_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var model = new ErrorViewModel();
            var requestId = "123";

            // Act
            model.RequestId = requestId;

            // Assert
            Assert.Equal(requestId, model.RequestId);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("123", true)]
        public void ShowRequestId_ReturnsCorrectValue(string requestId, bool expected)
        {
            // Arrange
            var model = new ErrorViewModel { RequestId = requestId };

            // Act
            var result = model.ShowRequestId;

            // Assert
            Assert.Equal(expected, result);
        }
    }
}