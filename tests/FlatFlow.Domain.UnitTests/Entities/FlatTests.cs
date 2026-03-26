using FlatFlow.Domain.Entities;
using FlatFlow.Domain.Enums;
using FlatFlow.Domain.Exceptions;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;

namespace FlatFlow.Domain.UnitTests.Entities
{
    public class FlatTests
    {
        private readonly Address _validAddress = new("Main St 1", "Warsaw", "00-001", "Poland");

        [Theory]
        [InlineData("My Flat")]
        [InlineData("Apartment 42")]
        [InlineData("A")]
        public void Constructor_WithValidName_SetsName(string name)
        {
            // Arrange & Act
            var flat = new Flat(name, _validAddress);

            // Assert
            flat.Name.Should().Be(name);
        }

        [Fact]
        public void Constructor_WithValidAddress_SetsAddress()
        {
            // Arrange & Act
            var flat = new Flat("My Flat", _validAddress);

            // Assert
            flat.Address.Should().Be(_validAddress);
        }

        [Fact]
        public void Constructor_WhenCalled_GeneratesNonEmptyId()
        {
            // Arrange & Act
            var flat = new Flat("My Flat", _validAddress);

            // Assert
            flat.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void Constructor_WhenCalled_SetsCreatedAtToUtcNow()
        {
            // Arrange
            var before = DateTime.UtcNow;

            // Act
            var flat = new Flat("My Flat", _validAddress);
            var after = DateTime.UtcNow;

            // Assert
            flat.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Fact]
        public void Constructor_WhenCalled_UpdatedAtIsNull()
        {
            // Arrange & Act
            var flat = new Flat("My Flat", _validAddress);

            // Assert
            flat.UpdatedAt.Should().BeNull();
        }

        [Fact]
        public void Constructor_WhenCalled_GeneratesAccessCodeWith8UppercaseChars()
        {
            // Arrange & Act
            var flat = new Flat("My Flat", _validAddress);

            // Assert
            flat.AccessCode.Should().HaveLength(8).And.BeUpperCased();
        }

        [Fact]
        public void Constructor_WhenCalled_InitializesEmptyCollections()
        {
            // Arrange & Act
            var flat = new Flat("My Flat", _validAddress);

            // Assert
            flat.Tenants.Should().BeEmpty();
            flat.Chores.Should().BeEmpty();
            flat.Notes.Should().BeEmpty();
            flat.Payments.Should().BeEmpty();
        }

        [Fact]
        public void Constructor_WhenCalledTwice_GeneratesUniqueIds()
        {
            // Arrange & Act
            var flat1 = new Flat("Flat 1", _validAddress);
            var flat2 = new Flat("Flat 2", _validAddress);

            // Assert
            flat1.Id.Should().NotBe(flat2.Id);
        }

        [Fact]
        public void Constructor_WhenCalledTwice_GeneratesUniqueAccessCodes()
        {
            // Arrange & Act
            var flat1 = new Flat("Flat 1", _validAddress);
            var flat2 = new Flat("Flat 2", _validAddress);

            // Assert
            flat1.AccessCode.Should().NotBe(flat2.AccessCode);
        }

        [Theory]
        [InlineData("New Name")]
        [InlineData("Updated Flat")]
        public void UpdateName_WithValue_ChangesName(string newName)
        {
            // Arrange
            var flat = new Flat("Original", _validAddress);

            // Act
            flat.UpdateName(newName);

            // Assert
            flat.Name.Should().Be(newName);
        }

        [Fact]
        public void UpdateName_WhenCalled_SetsUpdatedAt()
        {
            // Arrange
            var flat = new Flat("Original", _validAddress);
            var before = DateTime.UtcNow;

            // Act
            flat.UpdateName("New Name");
            var after = DateTime.UtcNow;

            // Assert
            flat.UpdatedAt.Should().NotBeNull();
            flat.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Fact]
        public void UpdateAddress_WithNewAddress_ChangesAddress()
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);
            var newAddress = new Address("New St 5", "Krakow", "30-001", "Poland");

            // Act
            flat.UpdateAddress(newAddress);

            // Assert
            flat.Address.Should().Be(newAddress);
        }

        [Fact]
        public void UpdateAddress_WhenCalled_SetsUpdatedAt()
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);
            var newAddress = new Address("New St 5", "Krakow", "30-001", "Poland");
            var before = DateTime.UtcNow;

            // Act
            flat.UpdateAddress(newAddress);
            var after = DateTime.UtcNow;

            // Assert
            flat.UpdatedAt.Should().NotBeNull();
            flat.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Fact]
        public void RefreshAccessCode_WhenCalled_ChangesAccessCode()
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);
            var originalCode = flat.AccessCode;

            // Act
            flat.RefreshAccessCode();

            // Assert
            flat.AccessCode.Should().NotBe(originalCode);
        }

        [Fact]
        public void RefreshAccessCode_WhenCalled_SetsUpdatedAt()
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);
            var before = DateTime.UtcNow;

            // Act
            flat.RefreshAccessCode();
            var after = DateTime.UtcNow;

            // Assert
            flat.UpdatedAt.Should().NotBeNull();
            flat.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Fact]
        public void RefreshAccessCode_WhenCalled_GeneratesValidAccessCode()
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);

            // Act
            flat.RefreshAccessCode();

            // Assert
            flat.AccessCode.Should().HaveLength(8).And.BeUpperCased();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidName_ThrowsDomainValidationException(string? name)
        {
            // Arrange & Act
            var act = () => new Flat(name!, _validAddress);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("Flat name cannot be empty.");
        }

        [Fact]
        public void Constructor_WithNullAddress_ThrowsDomainValidationException()
        {
            // Arrange & Act
            var act = () => new Flat("My Flat", null!);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("Address cannot be null.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void UpdateName_WithInvalidName_ThrowsDomainValidationException(string? name)
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);

            // Act
            var act = () => flat.UpdateName(name!);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("Flat name cannot be empty.");
        }

        [Fact]
        public void UpdateAddress_WithNull_ThrowsDomainValidationException()
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);

            // Act
            var act = () => flat.UpdateAddress(null!);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("Address cannot be null.");
        }


        [Fact]
        public void AddTenant_WithValidData_ReturnsTenantAndAddsToCollection()
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);

            // Act
            var tenant = flat.AddTenant("John", "Doe", "john@example.com", "user-123");

            // Assert
            tenant.Should().NotBeNull();
            tenant.FirstName.Should().Be("John");
            tenant.LastName.Should().Be("Doe");
            tenant.FlatId.Should().Be(flat.Id);
            flat.Tenants.Should().ContainSingle().Which.Should().Be(tenant);
        }

        [Fact]
        public void AddTenant_WithIsOwner_SetsIsOwnerToTrue()
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);

            // Act
            var tenant = flat.AddTenant("John", "Doe", "john@example.com", "user-123", isOwner: true);

            // Assert
            tenant.IsOwner.Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void AddTenant_WithInvalidFirstName_ThrowsDomainValidationException(string? firstName)
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);

            // Act
            var act = () => flat.AddTenant(firstName!, "Doe", "john@example.com", "user-123");

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("First name cannot be empty.");
        }

        [Fact]
        public void AddTenant_WithDuplicateUserId_ThrowsDomainException()
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);
            flat.AddTenant("John", "Doe", "john@example.com", "user-123");

            // Act
            var act = () => flat.AddTenant("Jane", "Doe", "jane@example.com", "user-123");

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("*user-123*already a tenant*");
        }


        [Fact]
        public void RemoveTenant_WithExistingId_RemovesFromCollection()
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);
            var tenant = flat.AddTenant("John", "Doe", "john@example.com", "user-123");

            // Act
            flat.RemoveTenant(tenant.Id);

            // Assert
            flat.Tenants.Should().BeEmpty();
        }

        [Fact]
        public void RemoveTenant_WithNonExistingId_ThrowsDomainException()
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);

            // Act
            var act = () => flat.RemoveTenant(Guid.NewGuid());

            // Assert
            act.Should().Throw<DomainException>();
        }


        [Fact]
        public void AddChore_WithValidData_ReturnsChoreAndAddsToCollection()
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);

            // Act
            var chore = flat.AddChore("Take out trash", "Use green bin", ChoreFrequency.Weekly);

            // Assert
            chore.Should().NotBeNull();
            chore.Title.Should().Be("Take out trash");
            chore.FlatId.Should().Be(flat.Id);
            flat.Chores.Should().ContainSingle().Which.Should().Be(chore);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void AddChore_WithInvalidTitle_ThrowsDomainValidationException(string? title)
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);

            // Act
            var act = () => flat.AddChore(title!, "Description", ChoreFrequency.Once);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("Chore title cannot be empty.");
        }


        [Fact]
        public void RemoveChore_WithExistingId_RemovesFromCollection()
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);
            var chore = flat.AddChore("Title", "Desc", ChoreFrequency.Once);

            // Act
            flat.RemoveChore(chore.Id);

            // Assert
            flat.Chores.Should().BeEmpty();
        }

        [Fact]
        public void RemoveChore_WithNonExistingId_ThrowsDomainException()
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);

            // Act
            var act = () => flat.RemoveChore(Guid.NewGuid());

            // Assert
            act.Should().Throw<DomainException>();
        }


        [Fact]
        public void AddNote_WithValidData_ReturnsNoteAndAddsToCollection()
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);
            var authorId = Guid.NewGuid();

            // Act
            var note = flat.AddNote("Shopping", "Buy milk", authorId);

            // Assert
            note.Should().NotBeNull();
            note.Title.Should().Be("Shopping");
            note.FlatId.Should().Be(flat.Id);
            note.AuthorId.Should().Be(authorId);
            flat.Notes.Should().ContainSingle().Which.Should().Be(note);
        }

        [Fact]
        public void AddNote_WithEmptyAuthorId_ThrowsDomainValidationException()
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);

            // Act
            var act = () => flat.AddNote("Title", "Content", Guid.Empty);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("Author ID cannot be empty.");
        }


        [Fact]
        public void RemoveNote_WithExistingId_RemovesFromCollection()
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);
            var note = flat.AddNote("Title", "Content", Guid.NewGuid());

            // Act
            flat.RemoveNote(note.Id);

            // Assert
            flat.Notes.Should().BeEmpty();
        }

        [Fact]
        public void RemoveNote_WithNonExistingId_ThrowsDomainException()
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);

            // Act
            var act = () => flat.RemoveNote(Guid.NewGuid());

            // Assert
            act.Should().Throw<DomainException>();
        }


        [Fact]
        public void AddPayment_WithValidData_ReturnsPaymentAndAddsToCollection()
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);
            var createdById = Guid.NewGuid();
            var dueDate = DateTime.UtcNow.AddDays(30);

            // Act
            var payment = flat.AddPayment("Rent", 1500m, dueDate, createdById);

            // Assert
            payment.Should().NotBeNull();
            payment.Title.Should().Be("Rent");
            payment.Amount.Should().Be(1500m);
            payment.FlatId.Should().Be(flat.Id);
            payment.CreatedById.Should().Be(createdById);
            flat.Payments.Should().ContainSingle().Which.Should().Be(payment);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void AddPayment_WithInvalidAmount_ThrowsDomainValidationException(decimal amount)
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);

            // Act
            var act = () => flat.AddPayment("Rent", amount, DateTime.UtcNow, Guid.NewGuid());

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("Payment amount must be greater than zero.");
        }

        [Fact]
        public void AddPayment_WithEmptyCreatedById_ThrowsDomainValidationException()
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);

            // Act
            var act = () => flat.AddPayment("Rent", 100m, DateTime.UtcNow, Guid.Empty);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("Created by ID cannot be empty.");
        }


        [Fact]
        public void RemovePayment_WithExistingId_RemovesFromCollection()
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);
            var payment = flat.AddPayment("Rent", 1500m, DateTime.UtcNow, Guid.NewGuid());

            // Act
            flat.RemovePayment(payment.Id);

            // Assert
            flat.Payments.Should().BeEmpty();
        }

        [Fact]
        public void RemovePayment_WithNonExistingId_ThrowsDomainException()
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);

            // Act
            var act = () => flat.RemovePayment(Guid.NewGuid());

            // Assert
            act.Should().Throw<DomainException>();
        }
    }
}
