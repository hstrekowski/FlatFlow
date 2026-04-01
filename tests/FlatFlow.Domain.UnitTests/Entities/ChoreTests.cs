using FlatFlow.Domain.Entities;
using FlatFlow.Domain.Enums;
using FlatFlow.Domain.Exceptions;
using FluentAssertions;

namespace FlatFlow.Domain.UnitTests.Entities
{
    public class ChoreTests
    {
        private readonly Guid _flatId = Guid.NewGuid();
        private readonly Guid _createdById = Guid.NewGuid();

        private Chore CreateChore()
            => new("Take out trash", "Use the green bin", ChoreFrequency.Weekly, _flatId, _createdById);

        [Theory]
        [InlineData("Take out trash")]
        [InlineData("Vacuum living room")]
        [InlineData("A")]
        public void Constructor_WithValidTitle_SetsTitle(string title)
        {
            // Arrange & Act
            var chore = new Chore(title, "Description", ChoreFrequency.Once, _flatId, _createdById);

            // Assert
            chore.Title.Should().Be(title);
        }

        [Theory]
        [InlineData("Use the green bin")]
        [InlineData("")]
        public void Constructor_WithDescription_SetsDescription(string description)
        {
            // Arrange & Act
            var chore = new Chore("Title", description, ChoreFrequency.Once, _flatId, _createdById);

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
            var chore = new Chore("Title", "Description", frequency, _flatId, _createdById);

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
        public void Constructor_WithCreatedById_SetsCreatedById()
        {
            // Arrange & Act
            var chore = CreateChore();

            // Assert
            chore.CreatedById.Should().Be(_createdById);
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
        public void Constructor_WhenCalled_UpdatedAtIsNull()
        {
            // Arrange & Act
            var chore = CreateChore();

            // Assert
            chore.UpdatedAt.Should().BeNull();
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
        [InlineData("New Title")]
        [InlineData("Updated")]
        public void UpdateTitle_WithNewValue_ChangesTitle(string title)
        {
            // Arrange
            var chore = CreateChore();

            // Act
            chore.UpdateTitle(title);

            // Assert
            chore.Title.Should().Be(title);
        }

        [Fact]
        public void UpdateTitle_WhenCalled_SetsUpdatedAt()
        {
            // Arrange
            var chore = CreateChore();
            var before = DateTime.UtcNow;

            // Act
            chore.UpdateTitle("New");
            var after = DateTime.UtcNow;

            // Assert
            chore.UpdatedAt.Should().NotBeNull();
            chore.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Theory]
        [InlineData("New Description")]
        [InlineData("")]
        public void UpdateDescription_WithNewValue_ChangesDescription(string description)
        {
            // Arrange
            var chore = CreateChore();

            // Act
            chore.UpdateDescription(description);

            // Assert
            chore.Description.Should().Be(description);
        }

        [Fact]
        public void UpdateDescription_WhenCalled_SetsUpdatedAt()
        {
            // Arrange
            var chore = CreateChore();
            var before = DateTime.UtcNow;

            // Act
            chore.UpdateDescription("New desc");
            var after = DateTime.UtcNow;

            // Assert
            chore.UpdatedAt.Should().NotBeNull();
            chore.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Theory]
        [InlineData(ChoreFrequency.Once)]
        [InlineData(ChoreFrequency.Daily)]
        [InlineData(ChoreFrequency.Monthly)]
        public void UpdateFrequency_WithNewValue_ChangesFrequency(ChoreFrequency frequency)
        {
            // Arrange
            var chore = CreateChore();

            // Act
            chore.UpdateFrequency(frequency);

            // Assert
            chore.Frequency.Should().Be(frequency);
        }

        [Fact]
        public void UpdateFrequency_WhenCalled_SetsUpdatedAt()
        {
            // Arrange
            var chore = CreateChore();
            var before = DateTime.UtcNow;

            // Act
            chore.UpdateFrequency(ChoreFrequency.Monthly);
            var after = DateTime.UtcNow;

            // Assert
            chore.UpdatedAt.Should().NotBeNull();
            chore.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidTitle_ThrowsDomainValidationException(string? title)
        {
            // Arrange & Act
            var act = () => new Chore(title!, "Description", ChoreFrequency.Once, _flatId, _createdById);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("Chore title cannot be empty.");
        }

        [Fact]
        public void Constructor_WithEmptyFlatId_ThrowsDomainValidationException()
        {
            // Arrange & Act
            var act = () => new Chore("Title", "Description", ChoreFrequency.Once, Guid.Empty, _createdById);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("Flat ID cannot be empty.");
        }

        [Fact]
        public void Constructor_WithEmptyCreatedById_ThrowsDomainValidationException()
        {
            // Arrange & Act
            var act = () => new Chore("Title", "Description", ChoreFrequency.Once, _flatId, Guid.Empty);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("Created by ID cannot be empty.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void UpdateTitle_WithInvalidTitle_ThrowsDomainValidationException(string? title)
        {
            // Arrange
            var chore = CreateChore();

            // Act
            var act = () => chore.UpdateTitle(title!);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("Chore title cannot be empty.");
        }


        [Fact]
        public void AddAssignment_WithValidData_ReturnsAssignmentAndAddsToCollection()
        {
            // Arrange
            var chore = CreateChore();
            var tenantId = Guid.NewGuid();
            var dueDate = DateTime.UtcNow.AddDays(7);

            // Act
            var assignment = chore.AddAssignment(tenantId, dueDate);

            // Assert
            assignment.Should().NotBeNull();
            assignment.TenantId.Should().Be(tenantId);
            assignment.DueDate.Should().Be(dueDate);
            assignment.ChoreId.Should().Be(chore.Id);
            chore.ChoreAssignments.Should().ContainSingle().Which.Should().Be(assignment);
        }

        [Fact]
        public void AddAssignment_WithEmptyTenantId_ThrowsDomainValidationException()
        {
            // Arrange
            var chore = CreateChore();

            // Act
            var act = () => chore.AddAssignment(Guid.Empty, DateTime.UtcNow.AddDays(7));

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("Tenant ID cannot be empty.");
        }

        [Fact]
        public void AddAssignment_WithDuplicateActiveTenant_ThrowsDomainException()
        {
            // Arrange
            var chore = CreateChore();
            var tenantId = Guid.NewGuid();
            chore.AddAssignment(tenantId, DateTime.UtcNow.AddDays(7));

            // Act
            var act = () => chore.AddAssignment(tenantId, DateTime.UtcNow.AddDays(14));

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("*already has an active assignment*");
        }

        [Fact]
        public void AddAssignment_WithCompletedDuplicateTenant_Succeeds()
        {
            // Arrange
            var chore = CreateChore();
            var tenantId = Guid.NewGuid();
            var firstAssignment = chore.AddAssignment(tenantId, DateTime.UtcNow.AddDays(7));
            firstAssignment.Complete();

            // Act
            var secondAssignment = chore.AddAssignment(tenantId, DateTime.UtcNow.AddDays(14));

            // Assert
            chore.ChoreAssignments.Should().HaveCount(2);
            secondAssignment.TenantId.Should().Be(tenantId);
        }


        [Fact]
        public void RemoveAssignment_WithExistingId_RemovesFromCollection()
        {
            // Arrange
            var chore = CreateChore();
            var assignment = chore.AddAssignment(Guid.NewGuid(), DateTime.UtcNow.AddDays(7));

            // Act
            chore.RemoveAssignment(assignment.Id);

            // Assert
            chore.ChoreAssignments.Should().BeEmpty();
        }

        [Fact]
        public void RemoveAssignment_WithNonExistingId_ThrowsDomainException()
        {
            // Arrange
            var chore = CreateChore();

            // Act
            var act = () => chore.RemoveAssignment(Guid.NewGuid());

            // Assert
            act.Should().Throw<DomainException>();
        }
    }
}
