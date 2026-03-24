using FlatFlow.Domain.Entities;
using FlatFlow.Domain.Exceptions;
using FluentAssertions;

namespace FlatFlow.Domain.UnitTests
{
    public class PaymentTests
    {
        private readonly Guid _flatId = Guid.NewGuid();
        private readonly Guid _createdById = Guid.NewGuid();
        private readonly DateTime _dueDate = DateTime.UtcNow.AddDays(30);

        private Payment CreatePayment()
            => new("Electricity bill", 150.50m, _dueDate, _flatId, _createdById);

        [Theory]
        [InlineData("Electricity bill")]
        [InlineData("Rent")]
        [InlineData("Groceries")]
        public void Constructor_WithValidTitle_SetsTitle(string title)
        {
            // Arrange & Act
            var payment = new Payment(title, 100m, _dueDate, _flatId, _createdById);

            // Assert
            payment.Title.Should().Be(title);
        }

        [Theory]
        [InlineData(99.99)]
        [InlineData(1500.00)]
        public void Constructor_WithValidAmount_SetsAmount(decimal amount)
        {
            // Arrange & Act
            var payment = new Payment("Title", amount, _dueDate, _flatId, _createdById);

            // Assert
            payment.Amount.Should().Be(amount);
        }

        [Fact]
        public void Constructor_WithFlatId_SetsFlatId()
        {
            // Arrange & Act
            var payment = CreatePayment();

            // Assert
            payment.FlatId.Should().Be(_flatId);
        }

        [Fact]
        public void Constructor_WithCreatedById_SetsCreatedById()
        {
            // Arrange & Act
            var payment = CreatePayment();

            // Assert
            payment.CreatedById.Should().Be(_createdById);
        }

        [Fact]
        public void Constructor_WithDueDate_SetsDueDate()
        {
            // Arrange & Act
            var payment = CreatePayment();

            // Assert
            payment.DueDate.Should().Be(_dueDate);
        }

        [Fact]
        public void Constructor_WhenCalled_GeneratesNonEmptyId()
        {
            // Arrange & Act
            var payment = CreatePayment();

            // Assert
            payment.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void Constructor_WhenCalled_SetsCreatedAtToUtcNow()
        {
            // Arrange
            var before = DateTime.UtcNow;

            // Act
            var payment = CreatePayment();
            var after = DateTime.UtcNow;

            // Assert
            payment.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Fact]
        public void Constructor_WhenCalled_UpdatedAtIsNull()
        {
            // Arrange & Act
            var payment = CreatePayment();

            // Assert
            payment.UpdatedAt.Should().BeNull();
        }

        [Fact]
        public void Constructor_WhenCalled_InitializesEmptyPaymentShares()
        {
            // Arrange & Act
            var payment = CreatePayment();

            // Assert
            payment.PaymentShares.Should().BeEmpty();
        }

        [Fact]
        public void Constructor_WhenCalledTwice_GeneratesUniqueIds()
        {
            // Arrange & Act
            var payment1 = CreatePayment();
            var payment2 = CreatePayment();

            // Assert
            payment1.Id.Should().NotBe(payment2.Id);
        }

        [Theory]
        [InlineData("New Title")]
        [InlineData("Updated")]
        public void UpdateTitle_WithNewValue_ChangesTitle(string title)
        {
            // Arrange
            var payment = CreatePayment();

            // Act
            payment.UpdateTitle(title);

            // Assert
            payment.Title.Should().Be(title);
        }

        [Fact]
        public void UpdateTitle_WhenCalled_SetsUpdatedAt()
        {
            // Arrange
            var payment = CreatePayment();
            var before = DateTime.UtcNow;

            // Act
            payment.UpdateTitle("New Title");
            var after = DateTime.UtcNow;

            // Assert
            payment.UpdatedAt.Should().NotBeNull();
            payment.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Theory]
        [InlineData(200.00)]
        [InlineData(999.99)]
        public void UpdateAmount_WithNewValue_ChangesAmount(decimal amount)
        {
            // Arrange
            var payment = CreatePayment();

            // Act
            payment.UpdateAmount(amount);

            // Assert
            payment.Amount.Should().Be(amount);
        }

        [Fact]
        public void UpdateAmount_WhenCalled_SetsUpdatedAt()
        {
            // Arrange
            var payment = CreatePayment();
            var before = DateTime.UtcNow;

            // Act
            payment.UpdateAmount(200m);
            var after = DateTime.UtcNow;

            // Assert
            payment.UpdatedAt.Should().NotBeNull();
            payment.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Fact]
        public void UpdateDueDate_WithNewValue_ChangesDueDate()
        {
            // Arrange
            var payment = CreatePayment();
            var newDueDate = _dueDate.AddDays(7);

            // Act
            payment.UpdateDueDate(newDueDate);

            // Assert
            payment.DueDate.Should().Be(newDueDate);
        }

        [Fact]
        public void UpdateDueDate_WhenCalled_SetsUpdatedAt()
        {
            // Arrange
            var payment = CreatePayment();
            var before = DateTime.UtcNow;

            // Act
            payment.UpdateDueDate(_dueDate.AddDays(7));
            var after = DateTime.UtcNow;

            // Assert
            payment.UpdatedAt.Should().NotBeNull();
            payment.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidTitle_ThrowsDomainValidationException(string? title)
        {
            // Arrange & Act
            var act = () => new Payment(title!, 100m, _dueDate, _flatId, _createdById);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("Payment title cannot be empty.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100.50)]
        public void Constructor_WithInvalidAmount_ThrowsDomainValidationException(decimal amount)
        {
            // Arrange & Act
            var act = () => new Payment("Title", amount, _dueDate, _flatId, _createdById);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("Payment amount must be greater than zero.");
        }

        [Fact]
        public void Constructor_WithEmptyFlatId_ThrowsDomainValidationException()
        {
            // Arrange & Act
            var act = () => new Payment("Title", 100m, _dueDate, Guid.Empty, _createdById);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("Flat ID cannot be empty.");
        }

        [Fact]
        public void Constructor_WithEmptyCreatedById_ThrowsDomainValidationException()
        {
            // Arrange & Act
            var act = () => new Payment("Title", 100m, _dueDate, _flatId, Guid.Empty);

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
            var payment = CreatePayment();

            // Act
            var act = () => payment.UpdateTitle(title!);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("Payment title cannot be empty.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100.50)]
        public void UpdateAmount_WithInvalidAmount_ThrowsDomainValidationException(decimal amount)
        {
            // Arrange
            var payment = CreatePayment();

            // Act
            var act = () => payment.UpdateAmount(amount);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("Payment amount must be greater than zero.");
        }

        // --- AddShare ---

        [Fact]
        public void AddShare_WithValidData_ReturnsShareAndAddsToCollection()
        {
            // Arrange
            var payment = CreatePayment();
            var tenantId = Guid.NewGuid();

            // Act
            var share = payment.AddShare(tenantId, 50m);

            // Assert
            share.Should().NotBeNull();
            share.TenantId.Should().Be(tenantId);
            share.ShareAmount.Should().Be(50m);
            share.PaymentId.Should().Be(payment.Id);
            payment.PaymentShares.Should().ContainSingle().Which.Should().Be(share);
        }

        [Fact]
        public void AddShare_WithEmptyTenantId_ThrowsDomainValidationException()
        {
            // Arrange
            var payment = CreatePayment();

            // Act
            var act = () => payment.AddShare(Guid.Empty, 50m);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("Tenant ID cannot be empty.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void AddShare_WithInvalidAmount_ThrowsDomainValidationException(decimal amount)
        {
            // Arrange
            var payment = CreatePayment();

            // Act
            var act = () => payment.AddShare(Guid.NewGuid(), amount);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("Share amount must be greater than zero.");
        }

        // --- RemoveShare ---

        [Fact]
        public void RemoveShare_WithExistingId_RemovesFromCollection()
        {
            // Arrange
            var payment = CreatePayment();
            var share = payment.AddShare(Guid.NewGuid(), 50m);

            // Act
            payment.RemoveShare(share.Id);

            // Assert
            payment.PaymentShares.Should().BeEmpty();
        }

        [Fact]
        public void RemoveShare_WithNonExistingId_ThrowsDomainException()
        {
            // Arrange
            var payment = CreatePayment();

            // Act
            var act = () => payment.RemoveShare(Guid.NewGuid());

            // Assert
            act.Should().Throw<DomainException>();
        }
    }
}
