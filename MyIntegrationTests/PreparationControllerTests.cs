using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using XAM.Controllers;
using XAM.Models;
using Xunit;
using static XAM.Models.HelperClass;

namespace MyIntegrationTests
{
    public class PreparationControllerTests : IDisposable
    {
        // Db setup

        private readonly XamDbContext _context;

        public PreparationControllerTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<XamDbContext>()
                .UseInMemoryDatabase(databaseName: "PreparationDatabase")
                .Options;

            _context = new XamDbContext(dbContextOptions, Mock.Of<IHttpContextAccessor>());
            _context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        // Tests

        [Fact]
        public void Preparation_ReturnsViewResult()
        {
            // Arrange
            var controller = new PreparationController(_context);

            // Act
            var result = controller.Preparation();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Theory]
        [InlineData("ValidExamName", "2023-12-04")]
        public void CreateExam_ValidInput_ReturnsJsonResult(string name, string date)
        {
            // Arrange
            var controller = new PreparationController(_context);

            // Act
            var result = controller.CreateExam(name, date) as JsonResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<JsonResult>(result);

            var data = result.Value as dynamic;
            Assert.NotNull(data);
            Assert.Equal($"{{ Name = {name}, Date = {DateTime.Parse(date).ToString("yyyy-MM-dd")} }}", data.ToString());
        }

        [Theory]
        [InlineData("Invalid_Exam_Name", "2023-12-04")]
        public void CreateExam_InvalidName_ReturnsJsonError(string name, string date)
        {
            // Arrange
            var controller = new PreparationController(_context);

            // Act
            var result = controller.CreateExam(name, date) as JsonResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<JsonResult>(result);

            var error = result.Value as ErrorRecord;
            Assert.NotNull(error);
            Assert.Equal("BadName", error.ErrorCode);
            Assert.Equal("Invalid exam name.", error.ErrorMessage);
        }

        [Theory]
        [InlineData("ValidExamName", "InvalidDate")]
        public void CreateExam_InvalidDate_ReturnsJsonError(string name, string date)
        {
            // Arrange
            var controller = new PreparationController(_context);

            // Act
            var result = controller.CreateExam(name, date) as JsonResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<JsonResult>(result);

            var error = result.Value as ErrorRecord;
            Assert.NotNull(error);
            Assert.Equal("BadDate", error.ErrorCode);
            Assert.Equal("Unparseable date.", error.ErrorMessage);
        }

        [Fact]
        public void CreateFlashcard_WithInvalidExam_ReturnsBadRequest()
        {
            // Arrange
            var controller = new PreparationController(_context);

            // Act
            var result = controller.CreateFlashcard("FrontText", "BackText", "NonexistentExam") as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Exam not found.", result.Value);
        }
    }
}
