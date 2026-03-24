using FlatFlow.Domain.Entities;
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
        [InlineData(0)]
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
        [InlineData("")]
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
        [InlineData(0)]
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
    }
}
