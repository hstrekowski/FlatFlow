using FlatFlow.Domain.Entities;
using FluentAssertions;

namespace FlatFlow.Domain.UnitTests
{
    public class NoteTests
    {
        private readonly Guid _flatId = Guid.NewGuid();
        private readonly Guid _authorId = Guid.NewGuid();

        private Note CreateNote()
            => new("Shopping List", "Buy milk and eggs", _flatId, _authorId);

        [Theory]
        [InlineData("Shopping List")]
        [InlineData("Reminder")]
        [InlineData("A")]
        public void Constructor_WithValidTitle_SetsTitle(string title)
        {
            // Arrange & Act
            var note = new Note(title, "Content", _flatId, _authorId);

            // Assert
            note.Title.Should().Be(title);
        }

        [Theory]
        [InlineData("Buy milk and eggs")]
        [InlineData("Don't forget to clean")]
        public void Constructor_WithValidContent_SetsContent(string content)
        {
            // Arrange & Act
            var note = new Note("Title", content, _flatId, _authorId);

            // Assert
            note.Content.Should().Be(content);
        }

        [Fact]
        public void Constructor_WithFlatId_SetsFlatId()
        {
            // Arrange & Act
            var note = CreateNote();

            // Assert
            note.FlatId.Should().Be(_flatId);
        }

        [Fact]
        public void Constructor_WithAuthorId_SetsAuthorId()
        {
            // Arrange & Act
            var note = CreateNote();

            // Assert
            note.AuthorId.Should().Be(_authorId);
        }

        [Fact]
        public void Constructor_WhenCalled_GeneratesNonEmptyId()
        {
            // Arrange & Act
            var note = CreateNote();

            // Assert
            note.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void Constructor_WhenCalled_SetsCreatedAtToUtcNow()
        {
            // Arrange
            var before = DateTime.UtcNow;

            // Act
            var note = CreateNote();
            var after = DateTime.UtcNow;

            // Assert
            note.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Fact]
        public void Constructor_WhenCalledTwice_GeneratesUniqueIds()
        {
            // Arrange & Act
            var note1 = CreateNote();
            var note2 = CreateNote();

            // Assert
            note1.Id.Should().NotBe(note2.Id);
        }

        [Theory]
        [InlineData("New Title", "New Content")]
        [InlineData("Updated", "Updated content here")]
        [InlineData("", "")]
        public void UpdateContent_WithNewValues_ChangesTitleAndContent(string title, string content)
        {
            // Arrange
            var note = CreateNote();

            // Act
            note.UpdateContent(title, content);

            // Assert
            note.Title.Should().Be(title);
            note.Content.Should().Be(content);
        }

        [Fact]
        public void UpdateContent_WhenCalled_DoesNotChangeOtherProperties()
        {
            // Arrange
            var note = CreateNote();

            // Act
            note.UpdateContent("New Title", "New Content");

            // Assert
            note.FlatId.Should().Be(_flatId);
            note.AuthorId.Should().Be(_authorId);
        }
    }
}
