using FlatFlow.Domain.Entities;
using FlatFlow.Domain.Exceptions;
using FluentAssertions;

namespace FlatFlow.Domain.UnitTests
{
    public class TenantTests
    {
        private readonly Guid _flatId = Guid.NewGuid();
        private const string _userId = "auth-user-123";

        private Tenant CreateTenant(bool isOwner = false)
            => new("John", "Doe", "john@example.com", _userId, _flatId, isOwner);

        [Theory]
        [InlineData("John", "Doe")]
        [InlineData("Anna", "Smith")]
        [InlineData("X", "Y")]
        public void Constructor_WithValidNames_SetsFirstAndLastName(string firstName, string lastName)
        {
            // Arrange & Act
            var tenant = new Tenant(firstName, lastName, "test@example.com", _userId, _flatId);

            // Assert
            tenant.FirstName.Should().Be(firstName);
            tenant.LastName.Should().Be(lastName);
        }

        [Fact]
        public void Constructor_WithValidEmail_SetsEmail()
        {
            // Arrange & Act
            var tenant = CreateTenant();

            // Assert
            tenant.Email.Should().Be("john@example.com");
        }

        [Fact]
        public void Constructor_WithUserId_SetsUserId()
        {
            // Arrange & Act
            var tenant = CreateTenant();

            // Assert
            tenant.UserId.Should().Be(_userId);
        }

        [Fact]
        public void Constructor_WithFlatId_SetsFlatId()
        {
            // Arrange & Act
            var tenant = CreateTenant();

            // Assert
            tenant.FlatId.Should().Be(_flatId);
        }

        [Fact]
        public void Constructor_WhenCalled_GeneratesNonEmptyId()
        {
            // Arrange & Act
            var tenant = CreateTenant();

            // Assert
            tenant.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void Constructor_WhenCalled_SetsCreatedAtToUtcNow()
        {
            // Arrange
            var before = DateTime.UtcNow;

            // Act
            var tenant = CreateTenant();
            var after = DateTime.UtcNow;

            // Assert
            tenant.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Fact]
        public void Constructor_WhenCalled_UpdatedAtIsNull()
        {
            // Arrange & Act
            var tenant = CreateTenant();

            // Assert
            tenant.UpdatedAt.Should().BeNull();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Constructor_WithIsOwnerFlag_SetsIsOwner(bool isOwner)
        {
            // Arrange & Act
            var tenant = CreateTenant(isOwner);

            // Assert
            tenant.IsOwner.Should().Be(isOwner);
        }

        [Fact]
        public void Constructor_WithoutIsOwnerFlag_DefaultsToFalse()
        {
            // Arrange & Act
            var tenant = new Tenant("John", "Doe", "john@example.com", _userId, _flatId);

            // Assert
            tenant.IsOwner.Should().BeFalse();
        }

        [Fact]
        public void Constructor_WhenCalled_InitializesEmptyCollections()
        {
            // Arrange & Act
            var tenant = CreateTenant();

            // Assert
            tenant.ChoreAssignments.Should().BeEmpty();
            tenant.PaymentShares.Should().BeEmpty();
            tenant.CreatedPayments.Should().BeEmpty();
            tenant.AuthoredNotes.Should().BeEmpty();
        }

        [Fact]
        public void Constructor_WhenCalledTwice_GeneratesUniqueIds()
        {
            // Arrange & Act
            var tenant1 = CreateTenant();
            var tenant2 = CreateTenant();

            // Assert
            tenant1.Id.Should().NotBe(tenant2.Id);
        }

        [Theory]
        [InlineData("Jane", "Smith")]
        [InlineData("Updated", "Name")]
        public void UpdateProfile_WithNewNames_ChangesFirstAndLastName(string firstName, string lastName)
        {
            // Arrange
            var tenant = CreateTenant();

            // Act
            tenant.UpdateProfile(firstName, lastName);

            // Assert
            tenant.FirstName.Should().Be(firstName);
            tenant.LastName.Should().Be(lastName);
        }

        [Fact]
        public void UpdateProfile_WhenCalled_SetsUpdatedAt()
        {
            // Arrange
            var tenant = CreateTenant();
            var before = DateTime.UtcNow;

            // Act
            tenant.UpdateProfile("Jane", "Smith");
            var after = DateTime.UtcNow;

            // Assert
            tenant.UpdatedAt.Should().NotBeNull();
            tenant.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Fact]
        public void UpdateProfile_WhenCalled_DoesNotChangeOtherProperties()
        {
            // Arrange
            var tenant = CreateTenant();

            // Act
            tenant.UpdateProfile("Jane", "Smith");

            // Assert
            tenant.Email.Should().Be("john@example.com");
            tenant.UserId.Should().Be(_userId);
            tenant.FlatId.Should().Be(_flatId);
        }

        [Fact]
        public void UpdateEmail_WhenCalled_SetsUpdatedAt()
        {
            // Arrange
            var tenant = CreateTenant();
            var before = DateTime.UtcNow;

            // Act
            tenant.UpdateEmail("new@example.com");
            var after = DateTime.UtcNow;

            // Assert
            tenant.UpdatedAt.Should().NotBeNull();
            tenant.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Theory]
        [InlineData("new@example.com")]
        [InlineData("updated@test.org")]
        public void UpdateEmail_WithNewEmail_ChangesEmail(string newEmail)
        {
            // Arrange
            var tenant = CreateTenant();

            // Act
            tenant.UpdateEmail(newEmail);

            // Assert
            tenant.Email.Should().Be(newEmail);
        }

        [Fact]
        public void PromoteToOwner_WhenNotOwner_SetsIsOwnerToTrue()
        {
            // Arrange
            var tenant = CreateTenant(isOwner: false);

            // Act
            tenant.PromoteToOwner();

            // Assert
            tenant.IsOwner.Should().BeTrue();
        }

        [Fact]
        public void PromoteToOwner_WhenNotOwner_SetsUpdatedAt()
        {
            // Arrange
            var tenant = CreateTenant(isOwner: false);
            var before = DateTime.UtcNow;

            // Act
            tenant.PromoteToOwner();
            var after = DateTime.UtcNow;

            // Assert
            tenant.UpdatedAt.Should().NotBeNull();
            tenant.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Fact]
        public void PromoteToOwner_WhenAlreadyOwner_ThrowsDomainException()
        {
            // Arrange
            var tenant = CreateTenant(isOwner: true);

            // Act
            var act = () => tenant.PromoteToOwner();

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Tenant is already an owner.");
        }

        [Fact]
        public void RevokeOwnership_WhenOwner_SetsIsOwnerToFalse()
        {
            // Arrange
            var tenant = CreateTenant(isOwner: true);

            // Act
            tenant.RevokeOwnership();

            // Assert
            tenant.IsOwner.Should().BeFalse();
        }

        [Fact]
        public void RevokeOwnership_WhenOwner_SetsUpdatedAt()
        {
            // Arrange
            var tenant = CreateTenant(isOwner: true);
            var before = DateTime.UtcNow;

            // Act
            tenant.RevokeOwnership();
            var after = DateTime.UtcNow;

            // Assert
            tenant.UpdatedAt.Should().NotBeNull();
            tenant.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Fact]
        public void RevokeOwnership_WhenNotOwner_ThrowsDomainException()
        {
            // Arrange
            var tenant = CreateTenant(isOwner: false);

            // Act
            var act = () => tenant.RevokeOwnership();

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Tenant is not an owner.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidFirstName_ThrowsDomainValidationException(string? firstName)
        {
            // Arrange & Act
            var act = () => new Tenant(firstName!, "Doe", "john@example.com", _userId, _flatId);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("First name cannot be empty.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidLastName_ThrowsDomainValidationException(string? lastName)
        {
            // Arrange & Act
            var act = () => new Tenant("John", lastName!, "john@example.com", _userId, _flatId);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("Last name cannot be empty.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidEmail_ThrowsDomainValidationException(string? email)
        {
            // Arrange & Act
            var act = () => new Tenant("John", "Doe", email!, _userId, _flatId);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("Email cannot be empty.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidUserId_ThrowsDomainValidationException(string? userId)
        {
            // Arrange & Act
            var act = () => new Tenant("John", "Doe", "john@example.com", userId!, _flatId);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("User ID cannot be empty.");
        }

        [Fact]
        public void Constructor_WithEmptyFlatId_ThrowsDomainValidationException()
        {
            // Arrange & Act
            var act = () => new Tenant("John", "Doe", "john@example.com", _userId, Guid.Empty);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("Flat ID cannot be empty.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void UpdateProfile_WithInvalidFirstName_ThrowsDomainValidationException(string? firstName)
        {
            // Arrange
            var tenant = CreateTenant();

            // Act
            var act = () => tenant.UpdateProfile(firstName!, "Smith");

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("First name cannot be empty.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void UpdateProfile_WithInvalidLastName_ThrowsDomainValidationException(string? lastName)
        {
            // Arrange
            var tenant = CreateTenant();

            // Act
            var act = () => tenant.UpdateProfile("Jane", lastName!);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("Last name cannot be empty.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void UpdateEmail_WithInvalidEmail_ThrowsDomainValidationException(string? email)
        {
            // Arrange
            var tenant = CreateTenant();

            // Act
            var act = () => tenant.UpdateEmail(email!);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("Email cannot be empty.");
        }
    }
}
