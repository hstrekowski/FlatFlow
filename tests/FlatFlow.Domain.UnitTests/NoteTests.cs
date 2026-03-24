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
            var note = new Note(title, "Content", _flatId, _authorId);

            note.Title.Should().Be(title);
        }

        [Theory]
        [InlineData("Buy milk and eggs")]
        [InlineData("Don't forget to clean")]
        public void Constructor_WithValidContent_SetsContent(string content)
        {
            var note = new Note("Title", content, _flatId, _authorId);

            note.Content.Should().Be(content);
        }

        [Fact]
        public void Constructor_WithFlatId_SetsFlatId()
        {
            var note = CreateNote();

            note.FlatId.Should().Be(_flatId);
        }

        [Fact]
        public void Constructor_WithAuthorId_SetsAuthorId()
        {
            var note = CreateNote();

            note.AuthorId.Should().Be(_authorId);
        }

        [Fact]
        public void Constructor_WhenCalled_GeneratesNonEmptyId()
        {
            var note = CreateNote();

            note.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void Constructor_WhenCalled_SetsCreatedAtToUtcNow()
        {
            var before = DateTime.UtcNow;
            var note = CreateNote();
            var after = DateTime.UtcNow;

            note.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Fact]
        public void Constructor_WhenCalledTwice_GeneratesUniqueIds()
        {
            var note1 = CreateNote();
            var note2 = CreateNote();

            note1.Id.Should().NotBe(note2.Id);
        }

        [Theory]
        [InlineData("New Title", "New Content")]
        [InlineData("Updated", "Updated content here")]
        [InlineData("", "")]
        public void UpdateContent_WithNewValues_ChangesTitleAndContent(string title, string content)
        {
            var note = CreateNote();

            note.UpdateContent(title, content);

            note.Title.Should().Be(title);
            note.Content.Should().Be(content);
        }

        [Fact]
        public void UpdateContent_WhenCalled_DoesNotChangeOtherProperties()
        {
            var note = CreateNote();

            note.UpdateContent("New Title", "New Content");

            note.FlatId.Should().Be(_flatId);
            note.AuthorId.Should().Be(_authorId);
        }
    }
}
