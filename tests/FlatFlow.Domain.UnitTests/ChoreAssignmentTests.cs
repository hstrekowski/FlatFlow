using FlatFlow.Domain.Entities;
using FluentAssertions;

namespace FlatFlow.Domain.UnitTests
{
    public class ChoreAssignmentTests
    {
        private readonly Guid _tenantId = Guid.NewGuid();
        private readonly Guid _choreId = Guid.NewGuid();

        private ChoreAssignment CreateAssignment()
            => new(_tenantId, _choreId);

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
    }
}
