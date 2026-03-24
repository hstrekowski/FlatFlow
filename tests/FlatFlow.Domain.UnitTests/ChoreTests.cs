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
            var chore = new Chore(title, "Description", ChoreFrequency.Once, _flatId);

            chore.Title.Should().Be(title);
        }

        [Theory]
        [InlineData("Use the green bin")]
        [InlineData("")]
        public void Constructor_WithDescription_SetsDescription(string description)
        {
            var chore = new Chore("Title", description, ChoreFrequency.Once, _flatId);

            chore.Description.Should().Be(description);
        }

        [Theory]
        [InlineData(ChoreFrequency.Once)]
        [InlineData(ChoreFrequency.Daily)]
        [InlineData(ChoreFrequency.Weekly)]
        [InlineData(ChoreFrequency.Monthly)]
        public void Constructor_WithFrequency_SetsFrequency(ChoreFrequency frequency)
        {
            var chore = new Chore("Title", "Description", frequency, _flatId);

            chore.Frequency.Should().Be(frequency);
        }

        [Fact]
        public void Constructor_WithFlatId_SetsFlatId()
        {
            var chore = CreateChore();

            chore.FlatId.Should().Be(_flatId);
        }

        [Fact]
        public void Constructor_WhenCalled_GeneratesNonEmptyId()
        {
            var chore = CreateChore();

            chore.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void Constructor_WhenCalled_SetsCreatedAtToUtcNow()
        {
            var before = DateTime.UtcNow;
            var chore = CreateChore();
            var after = DateTime.UtcNow;

            chore.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Fact]
        public void Constructor_WhenCalled_InitializesEmptyChoreAssignments()
        {
            var chore = CreateChore();

            chore.ChoreAssignments.Should().BeEmpty();
        }

        [Fact]
        public void Constructor_WhenCalledTwice_GeneratesUniqueIds()
        {
            var chore1 = CreateChore();
            var chore2 = CreateChore();

            chore1.Id.Should().NotBe(chore2.Id);
        }

        [Theory]
        [InlineData("New Title", "New Description", ChoreFrequency.Daily)]
        [InlineData("Updated", "", ChoreFrequency.Monthly)]
        public void Update_WithNewValues_ChangesProperties(string title, string description, ChoreFrequency frequency)
        {
            var chore = CreateChore();

            chore.Update(title, description, frequency);

            chore.Title.Should().Be(title);
            chore.Description.Should().Be(description);
            chore.Frequency.Should().Be(frequency);
        }

        [Fact]
        public void Update_WhenCalled_DoesNotChangeOtherProperties()
        {
            var chore = CreateChore();

            chore.Update("New", "New desc", ChoreFrequency.Monthly);

            chore.FlatId.Should().Be(_flatId);
        }
    }
}
