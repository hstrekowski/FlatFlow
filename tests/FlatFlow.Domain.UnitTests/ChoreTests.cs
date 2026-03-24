using FlatFlow.Domain.Entities;
using FlatFlow.Domain.Enums;
using FluentAssertions;

namespace FlatFlow.Domain.UnitTests
{
    public class ChoreTests
    {
        private readonly Guid _flatId = Guid.NewGuid();

        private Chore CreateChore()
            => new("Take out trash", "Use the green bin", ChoreFrequency.Weekly, _flatId);

        [Theory]
        [InlineData("Take out trash")]
        [InlineData("Vacuum living room")]
        [InlineData("A")]
        public void Constructor_WithValidTitle_SetsTitle(string title)
        {
            // Arrange & Act
            var chore = new Chore(title, "Description", ChoreFrequency.Once, _flatId);

            // Assert
            chore.Title.Should().Be(title);
        }

        [Theory]
        [InlineData("Use the green bin")]
        [InlineData("")]
        public void Constructor_WithDescription_SetsDescription(string description)
        {
            // Arrange & Act
            var chore = new Chore("Title", description, ChoreFrequency.Once, _flatId);

            // Assert
            chore.Description.Should().Be(description);
        }

        [Theory]
        [InlineData(ChoreFrequency.Once)]
        [InlineData(ChoreFrequency.Daily)]
        [InlineData(ChoreFrequency.Weekly)]
        [InlineData(ChoreFrequency.Monthly)]
        public void Constructor_WithFrequency_SetsFrequency(ChoreFrequency frequency)
        {
            // Arrange & Act
            var chore = new Chore("Title", "Description", frequency, _flatId);

            // Assert
            chore.Frequency.Should().Be(frequency);
        }

        [Fact]
        public void Constructor_WithFlatId_SetsFlatId()
        {
            // Arrange & Act
            var chore = CreateChore();

            // Assert
            chore.FlatId.Should().Be(_flatId);
        }

        [Fact]
        public void Constructor_WhenCalled_GeneratesNonEmptyId()
        {
            // Arrange & Act
            var chore = CreateChore();

            // Assert
            chore.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void Constructor_WhenCalled_SetsCreatedAtToUtcNow()
        {
            // Arrange
            var before = DateTime.UtcNow;

            // Act
            var chore = CreateChore();
            var after = DateTime.UtcNow;

            // Assert
            chore.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Fact]
        public void Constructor_WhenCalled_InitializesEmptyChoreAssignments()
        {
            // Arrange & Act
            var chore = CreateChore();

            // Assert
            chore.ChoreAssignments.Should().BeEmpty();
        }

        [Fact]
        public void Constructor_WhenCalledTwice_GeneratesUniqueIds()
        {
            // Arrange & Act
            var chore1 = CreateChore();
            var chore2 = CreateChore();

            // Assert
            chore1.Id.Should().NotBe(chore2.Id);
        }

        [Theory]
        [InlineData("New Title", "New Description", ChoreFrequency.Daily)]
        [InlineData("Updated", "", ChoreFrequency.Monthly)]
        public void Update_WithNewValues_ChangesProperties(string title, string description, ChoreFrequency frequency)
        {
            // Arrange
            var chore = CreateChore();

            // Act
            chore.Update(title, description, frequency);

            // Assert
            chore.Title.Should().Be(title);
            chore.Description.Should().Be(description);
            chore.Frequency.Should().Be(frequency);
        }

        [Fact]
        public void Update_WhenCalled_DoesNotChangeOtherProperties()
        {
            // Arrange
            var chore = CreateChore();

            // Act
            chore.Update("New", "New desc", ChoreFrequency.Monthly);

            // Assert
            chore.FlatId.Should().Be(_flatId);
        }
    }
}
