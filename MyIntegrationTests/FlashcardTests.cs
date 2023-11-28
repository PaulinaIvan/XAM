using Xunit;
using XAM.Models;

namespace FlashcardTests
{
    public class FlashcardTests
    {
        [Fact]
        public void Flashcard_Constructor_SetsPropertiesCorrectly()
        {
            // Arrange
            var frontText = "Front Text";
            var backText = "Back Text";

            // Act
            var flashcard = new Flashcard(frontText, backText);

            // Assert
            Assert.Equal(frontText, flashcard.FrontText);
            Assert.Equal(backText, flashcard.BackText);
        }

        [Fact]
        public void Flashcard_CompareTo_ReturnsCorrectOrder()
        {
            // Arrange
            var flashcard1 = new Flashcard("A", "Back Text");
            var flashcard2 = new Flashcard("B", "Back Text");

            // Act
            var result = flashcard1.CompareTo(flashcard2);

            // Assert
            Assert.True(result < 0);
        }

        [Fact]
        public void Flashcard_CompareTo_HandlesNullCorrectly()
        {
            // Arrange
            var flashcard = new Flashcard("A", "Back Text");

            // Act
            var result = flashcard.CompareTo(null);

            // Assert
            Assert.True(result > 0);
        }
    }
}