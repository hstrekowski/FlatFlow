using FlatFlow.Domain.Entities;
using FlatFlow.Domain.Exceptions;
using FluentAssertions;

namespace FlatFlow.Domain.UnitTests.Entities
{
    public class ChoreAssignmentTests
    {
        private readonly Guid _tenantId = Guid.NewGuid();
        private readonly Guid _choreId = Guid.NewGuid();
        private readonly DateTime _dueDate = DateTime.UtcNow.AddDays(7);

        private ChoreAssignment CreateAssignment()
            => new(_tenantId, _choreId, _dueDate);

        [Fact]
        public void Constructor_WithTenantId_SetsTenantId()
        {
            // Arrange & Act
            var assignment = CreateAssignment();

            // Assert
            assignment.TenantId.Should().Be(_tenantId);
        }

        [Fact]
        public void Constructor_WithChoreId_SetsChoreId()
        {
            // Arrange & Act
            var assignment = CreateAssignment();

            // Assert
            assignment.ChoreId.Should().Be(_choreId);
        }

        [Fact]
        public void Constructor_WithDueDate_SetsDueDate()
        {
            // Arrange & Act
            var assignment = CreateAssignment();

            // Assert
            assignment.DueDate.Should().Be(_dueDate);
        }

        [Fact]
        public void Constructor_WhenCalled_GeneratesNonEmptyId()
        {
            // Arrange & Act
            var assignment = CreateAssignment();

            // Assert
            assignment.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void Constructor_WhenCalled_SetsCreatedAtToUtcNow()
        {
            // Arrange
            var before = DateTime.UtcNow;

            // Act
            var assignment = CreateAssignment();
            var after = DateTime.UtcNow;

            // Assert
            assignment.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Fact]
        public void Constructor_WhenCalled_UpdatedAtIsNull()
        {
            // Arrange & Act
            var assignment = CreateAssignment();

            // Assert
            assignment.UpdatedAt.Should().BeNull();
        }

        [Fact]
        public void Constructor_WhenCalled_CompletedAtIsNull()
        {
            // Arrange & Act
            var assignment = CreateAssignment();

            // Assert
            assignment.CompletedAt.Should().BeNull();
        }

        [Fact]
        public void Constructor_WhenCalledTwice_GeneratesUniqueIds()
        {
            // Arrange & Act
            var assignment1 = CreateAssignment();
            var assignment2 = CreateAssignment();

            // Assert
            assignment1.Id.Should().NotBe(assignment2.Id);
        }

        [Fact]
        public void UpdateDueDate_WithNewDate_ChangesDueDate()
        {
            // Arrange
            var assignment = CreateAssignment();
            var newDueDate = DateTime.UtcNow.AddDays(14);

            // Act
            assignment.UpdateDueDate(newDueDate);

            // Assert
            assignment.DueDate.Should().Be(newDueDate);
        }

        [Fact]
        public void UpdateDueDate_WhenCalled_SetsUpdatedAt()
        {
            // Arrange
            var assignment = CreateAssignment();
            var before = DateTime.UtcNow;

            // Act
            assignment.UpdateDueDate(DateTime.UtcNow.AddDays(14));
            var after = DateTime.UtcNow;

            // Assert
            assignment.UpdatedAt.Should().NotBeNull();
            assignment.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Fact]
        public void UpdateDueDate_WhenCalled_DoesNotChangeOtherProperties()
        {
            // Arrange
            var assignment = CreateAssignment();

            // Act
            assignment.UpdateDueDate(DateTime.UtcNow.AddDays(14));

            // Assert
            assignment.TenantId.Should().Be(_tenantId);
            assignment.ChoreId.Should().Be(_choreId);
            assignment.CompletedAt.Should().BeNull();
        }

        [Fact]
        public void Complete_WhenCalled_SetsCompletedAt()
        {
            // Arrange
            var assignment = CreateAssignment();
            var before = DateTime.UtcNow;

            // Act
            assignment.Complete();
            var after = DateTime.UtcNow;

            // Assert
            assignment.CompletedAt.Should().NotBeNull();
            assignment.CompletedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Fact]
        public void Complete_WhenCalled_SetsUpdatedAt()
        {
            // Arrange
            var assignment = CreateAssignment();
            var before = DateTime.UtcNow;

            // Act
            assignment.Complete();
            var after = DateTime.UtcNow;

            // Assert
            assignment.UpdatedAt.Should().NotBeNull();
            assignment.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Fact]
        public void Complete_WhenCalled_DoesNotChangeOtherProperties()
        {
            // Arrange
            var assignment = CreateAssignment();

            // Act
            assignment.Complete();

            // Assert
            assignment.TenantId.Should().Be(_tenantId);
            assignment.ChoreId.Should().Be(_choreId);
        }

        [Fact]
        public void Complete_WhenAlreadyCompleted_ThrowsDomainException()
        {
            // Arrange
            var assignment = CreateAssignment();
            assignment.Complete();

            // Act
            var act = () => assignment.Complete();

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Chore assignment is already completed.");
        }

        [Fact]
        public void IsCompleted_WhenNotCompleted_ReturnsFalse()
        {
            // Arrange & Act
            var assignment = CreateAssignment();

            // Assert
            assignment.IsCompleted.Should().BeFalse();
        }

        [Fact]
        public void IsCompleted_WhenCompleted_ReturnsTrue()
        {
            // Arrange
            var assignment = CreateAssignment();

            // Act
            assignment.Complete();

            // Assert
            assignment.IsCompleted.Should().BeTrue();
        }

        [Fact]
        public void Reopen_WhenCompleted_SetsCompletedAtToNull()
        {
            // Arrange
            var assignment = CreateAssignment();
            assignment.Complete();

            // Act
            assignment.Reopen();

            // Assert
            assignment.CompletedAt.Should().BeNull();
            assignment.IsCompleted.Should().BeFalse();
        }

        [Fact]
        public void Reopen_WhenCompleted_SetsUpdatedAt()
        {
            // Arrange
            var assignment = CreateAssignment();
            assignment.Complete();
            var before = DateTime.UtcNow;

            // Act
            assignment.Reopen();
            var after = DateTime.UtcNow;

            // Assert
            assignment.UpdatedAt.Should().NotBeNull();
            assignment.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Fact]
        public void Reopen_WhenNotCompleted_ThrowsDomainException()
        {
            // Arrange
            var assignment = CreateAssignment();

            // Act
            var act = () => assignment.Reopen();

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Chore assignment is not completed.");
        }

        [Fact]
        public void Constructor_WithEmptyTenantId_ThrowsDomainValidationException()
        {
            // Arrange & Act
            var act = () => new ChoreAssignment(Guid.Empty, _choreId, _dueDate);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("Tenant ID cannot be empty.");
        }

        [Fact]
        public void Constructor_WithEmptyChoreId_ThrowsDomainValidationException()
        {
            // Arrange & Act
            var act = () => new ChoreAssignment(_tenantId, Guid.Empty, _dueDate);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("Chore ID cannot be empty.");
        }
    }
}
