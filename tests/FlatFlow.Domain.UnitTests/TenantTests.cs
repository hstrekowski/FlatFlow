using FlatFlow.Domain.Entities;
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
            var tenant = new Tenant(firstName, lastName, "test@example.com", _userId, _flatId);

            tenant.FirstName.Should().Be(firstName);
            tenant.LastName.Should().Be(lastName);
        }

        [Fact]
        public void Constructor_WithValidEmail_SetsEmail()
        {
            var tenant = CreateTenant();

            tenant.Email.Should().Be("john@example.com");
        }

        [Fact]
        public void Constructor_WithUserId_SetsUserId()
        {
            var tenant = CreateTenant();

            tenant.UserId.Should().Be(_userId);
        }

        [Fact]
        public void Constructor_WithFlatId_SetsFlatId()
        {
            var tenant = CreateTenant();

            tenant.FlatId.Should().Be(_flatId);
        }

        [Fact]
        public void Constructor_WhenCalled_GeneratesNonEmptyId()
        {
            var tenant = CreateTenant();

            tenant.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void Constructor_WhenCalled_SetsCreatedAtToUtcNow()
        {
            var before = DateTime.UtcNow;
            var tenant = CreateTenant();
            var after = DateTime.UtcNow;

            tenant.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Constructor_WithIsOwnerFlag_SetsIsOwner(bool isOwner)
        {
            var tenant = CreateTenant(isOwner);

            tenant.IsOwner.Should().Be(isOwner);
        }

        [Fact]
        public void Constructor_WithoutIsOwnerFlag_DefaultsToFalse()
        {
            var tenant = new Tenant("John", "Doe", "john@example.com", _userId, _flatId);

            tenant.IsOwner.Should().BeFalse();
        }

        [Fact]
        public void Constructor_WhenCalled_InitializesEmptyCollections()
        {
            var tenant = CreateTenant();

            tenant.ChoreAssignments.Should().BeEmpty();
            tenant.PaymentShares.Should().BeEmpty();
        }

        [Fact]
        public void Constructor_WhenCalledTwice_GeneratesUniqueIds()
        {
            var tenant1 = CreateTenant();
            var tenant2 = CreateTenant();

            tenant1.Id.Should().NotBe(tenant2.Id);
        }

        [Theory]
        [InlineData("Jane", "Smith")]
        [InlineData("Updated", "Name")]
        [InlineData("", "")]
        public void UpdateProfile_WithNewNames_ChangesFirstAndLastName(string firstName, string lastName)
        {
            var tenant = CreateTenant();

            tenant.UpdateProfile(firstName, lastName);

            tenant.FirstName.Should().Be(firstName);
            tenant.LastName.Should().Be(lastName);
        }

        [Fact]
        public void UpdateProfile_WhenCalled_DoesNotChangeOtherProperties()
        {
            var tenant = CreateTenant();

            tenant.UpdateProfile("Jane", "Smith");

            tenant.Email.Should().Be("john@example.com");
            tenant.UserId.Should().Be(_userId);
            tenant.FlatId.Should().Be(_flatId);
        }

        [Theory]
        [InlineData("new@example.com")]
        [InlineData("updated@test.org")]
        public void UpdateEmail_WithNewEmail_ChangesEmail(string newEmail)
        {
            var tenant = CreateTenant();

            tenant.UpdateEmail(newEmail);

            tenant.Email.Should().Be(newEmail);
        }
    }
}
