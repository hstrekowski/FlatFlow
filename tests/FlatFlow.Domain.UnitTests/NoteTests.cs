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
        public void Constructor_WhenCalled_UpdatedAtIsNull()
        {
            // Arrange & Act
            var note = CreateNote();

            // Assert
            note.UpdatedAt.Should().BeNull();
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
        [InlineData("New Title")]
        [InlineData("Updated")]
        public void UpdateTitle_WithNewValue_ChangesTitle(string title)
        {
            // Arrange
            var note = CreateNote();

            // Act
            note.UpdateTitle(title);

            // Assert
            note.Title.Should().Be(title);
        }

        [Fact]
        public void UpdateTitle_WhenCalled_SetsUpdatedAt()
        {
            // Arrange
            var note = CreateNote();
            var before = DateTime.UtcNow;

            // Act
            note.UpdateTitle("New Title");
            var after = DateTime.UtcNow;

            // Assert
            note.UpdatedAt.Should().NotBeNull();
            note.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Theory]
        [InlineData("New Content")]
        [InlineData("Updated content here")]
        [InlineData("")]
        public void UpdateContent_WithNewValue_ChangesContent(string content)
        {
            // Arrange
            var note = CreateNote();

            // Act
            note.UpdateContent(content);

            // Assert
            note.Content.Should().Be(content);
        }

        [Fact]
        public void UpdateContent_WhenCalled_SetsUpdatedAt()
        {
            // Arrange
            var note = CreateNote();
            var before = DateTime.UtcNow;

            // Act
            note.UpdateContent("New Content");
            var after = DateTime.UtcNow;

            // Assert
            note.UpdatedAt.Should().NotBeNull();
            note.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidTitle_ThrowsArgumentException(string? title)
        {
            // Arrange & Act
            var act = () => new Note(title!, "Content", _flatId, _authorId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Note title cannot be empty.*");
        }

        [Fact]
        public void Constructor_WithEmptyFlatId_ThrowsArgumentException()
        {
            // Arrange & Act
            var act = () => new Note("Title", "Content", Guid.Empty, _authorId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Flat ID cannot be empty.*");
        }

        [Fact]
        public void Constructor_WithEmptyAuthorId_ThrowsArgumentException()
        {
            // Arrange & Act
            var act = () => new Note("Title", "Content", _flatId, Guid.Empty);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Author ID cannot be empty.*");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void UpdateTitle_WithInvalidTitle_ThrowsArgumentException(string? title)
        {
            // Arrange
            var note = CreateNote();

            // Act
            var act = () => note.UpdateTitle(title!);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Note title cannot be empty.*");
        }
    }
}
