using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Moq;
using Xunit;
using XAM.Controllers;
using XAM.Models;
using static XAM.Models.HelperClass;

namespace MyIntegrationTests
{
    public class TasksControllerTests
    {
        private readonly DbContextOptions<XamDbContext> _options;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;

        public TasksControllerTests()
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
        public void Tasks_ReturnsViewResult()
        {
            // Arrange
            var controller = new TasksController(new XamDbContext(_options, _mockHttpContextAccessor.Object));

            // Act
            var result = controller.Tasks();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void FetchExamNames_ReturnsValidResult()
        {
            // Arrange
            var controller = new TasksController(new XamDbContext(_options, _mockHttpContextAccessor.Object));

            // Act
            var result = controller.FetchExamNames();

            // Assert
            Assert.IsType<JsonResult>(result);
        }

        /* Doesn't work
        [Fact]
        public void FetchFlashcardsOfExam_ReturnsFlashcards()
        {
            // Arrange
            var context = new XamDbContext(_options, _mockHttpContextAccessor.Object);
            var controller = new TasksController(context);
            var dataHolder = new DataHolder { OwnerUsername = "testUser" };
            var exam = new Exam("testExam", DateTime.UtcNow);

            List<Flashcard> flashcardList = new();
            for(int i = 0; i < 12; ++i)
                flashcardList.Add(new Flashcard(i.ToString(), i.ToString()));

            exam.Flashcards.AddRange(flashcardList);
            dataHolder.Exams.Add(exam);
            context.DataHoldersTable.Add(dataHolder);
            context.SaveChanges();

            // Act
            var result = controller.FetchFlashcardsOfExam("testExam") as JsonResult;
            var data = result.Value as dynamic;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<JsonResult>(result);

            Assert.NotNull(data);
            Assert.Equal("a", data.ToString());
        }
        */

        [Fact]
        public void FetchFlashcardsOfExam_ReturnsErrorResult()
        {
            // Arrange
            var context = new XamDbContext(_options, _mockHttpContextAccessor.Object);
            var controller = new TasksController(context);
            var dataHolder = new DataHolder { OwnerUsername = "testUser" };
            var exam = new Exam("testExam", DateTime.Now);
            dataHolder.Exams.Add(exam);
            context.DataHoldersTable.Add(dataHolder);
            context.SaveChanges();

            // Act
            var result = controller.FetchFlashcardsOfExam("testExam") as JsonResult;
            var error = result?.Value as ErrorRecord;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<JsonResult>(result);

            Assert.NotNull(error);
            Assert.Equal("NoFlashcards", error.ErrorCode);
            Assert.Equal("No flashcards for exam testExam found.", error.ErrorMessage);
        }

        [Fact]
        public void SetChallengeHighscoreForExam_ReturnsErrorResult()
        {
            // Arrange
            var context = new XamDbContext(_options, _mockHttpContextAccessor.Object);
            var controller = new TasksController(context);
            var dataHolder = new DataHolder { OwnerUsername = "testUser" };
            context.DataHoldersTable.Add(dataHolder);
            context.SaveChanges();

            // Act
            var result = controller.SetChallengeHighscoreForExam("nonExistentExam", 5) as JsonResult;
            var error = result?.Value as ErrorRecord;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<JsonResult>(result);

            Assert.NotNull(error);
            Assert.Equal("NoExamWithName", error.ErrorCode);
            Assert.Equal("Exam with name nonExistentExam no longer exists.", error.ErrorMessage);
        }

        /* Doesn't work
        [Fact]
        public void SetChallengeHighscoreForExam_ReturnsJsonResult()
        {
            // Arrange
            var context = new XamDbContext(_options, _mockHttpContextAccessor.Object);
            var controller = new TasksController(context);
            var dataHolder = new DataHolder { OwnerUsername = "testUser" };
            var exam = new Exam("testExam", DateTime.Now);
            dataHolder.Exams.Add(exam);
            context.DataHoldersTable.Add(dataHolder);
            context.SaveChanges();

            // Act
            var result = controller.SetChallengeHighscoreForExam("testExam", 5) as JsonResult;
            var data = result.Value as dynamic;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<JsonResult>(result);

            Assert.NotNull(data);
            Assert.Equal("a", data.ToString());
        }
        */
    }
}
