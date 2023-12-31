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
    public class PreparationControllerTests
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

        // Tests

        [Fact]
        public void Preparation_ReturnsViewResult()
        {
            // Arrange
            var controller = new PreparationController(CreateDatabaseContext());

            // Act
            var result = controller.Preparation();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void FetchExams_ReturnsJsonResult()
        {
            // Arrange
            var controller = new PreparationController(CreateDatabaseContext());

            // Act
            var result = controller.FetchExams() as JsonResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<JsonResult>(result);
        }

        [Theory]
        [InlineData("ValidExamName", "2023-12-04")]
        public void CreateExam_ValidInput_ReturnsJsonResult(string name, string date)
        {
            // Arrange
            var controller = new PreparationController(CreateDatabaseContext());

            // Act
            var result = controller.CreateExam(name, date) as JsonResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<JsonResult>(result);

            var data = result.Value as dynamic;
            Assert.NotNull(data);
            Assert.Equal($"{{ Name = {name}, Date = {DateTime.Parse(date).ToString("yyyy-MM-dd")} }}", data?.ToString());
        }

        [Theory]
        [InlineData("Invalid_Exam_Name", "2023-12-04")]
        public void CreateExam_InvalidName_ReturnsJsonError(string name, string date)
        {
            // Arrange
            var controller = new PreparationController(CreateDatabaseContext());

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
            var controller = new PreparationController(CreateDatabaseContext());

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
            var controller = new PreparationController(CreateDatabaseContext());

            // Act
            var result = controller.CreateFlashcard("FrontText", "BackText", "NonexistentExam") as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Exam not found.", result.Value);
        }

        [Fact]
        public void GetExamsNotOldDataHolder_ReturnsExamsNotInOldDataHolder()
        {
            // Arrange
            var oldDataHolder = new DataHolder { Exams = new List<Exam> { new Exam("Math", DateTime.Now), new Exam("English", DateTime.Now) } };
            var newDataHolder = new DataHolder { Exams = new List<Exam> { new Exam("Math", DateTime.Now), new Exam("Science", DateTime.Now) } };

            // Act
            var result = PreparationController.GetExamsNotOldDataHolder(oldDataHolder, newDataHolder);

            // Assert
            Assert.Collection(result,
                exam => Assert.Equal("Science", exam.Name)
            );
        }
    }
}
