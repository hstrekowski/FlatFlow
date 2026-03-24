using FlatFlow.Domain.Entities;
using FluentAssertions;

namespace FlatFlow.Domain.UnitTests
{
    public class PaymentTests
    {
        private readonly Guid _flatId = Guid.NewGuid();
        private readonly Guid _createdById = Guid.NewGuid();

        private Payment CreatePayment()
            => new("Electricity bill", 150.50m, _flatId, _createdById);

        [Theory]
        [InlineData("Electricity bill")]
        [InlineData("Rent")]
        [InlineData("Groceries")]
        public void Constructor_WithValidTitle_SetsTitle(string title)
        {
            // Arrange & Act
            var payment = new Payment(title, 100m, _flatId, _createdById);

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
            var payment = new Payment("Title", amount, _flatId, _createdById);

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
        [InlineData("New Title", 200.00)]
        [InlineData("Updated", 0)]
        public void Update_WithNewValues_ChangesTitleAndAmount(string title, decimal amount)
        {
            // Arrange
            var payment = CreatePayment();

            // Act
            payment.Update(title, amount);

            // Assert
            payment.Title.Should().Be(title);
            payment.Amount.Should().Be(amount);
        }

        [Fact]
        public void Update_WhenCalled_SetsUpdatedAt()
        {
            // Arrange
            var payment = CreatePayment();
            var before = DateTime.UtcNow;

            // Act
            payment.Update("New Title", 200m);
            var after = DateTime.UtcNow;

            // Assert
            payment.UpdatedAt.Should().NotBeNull();
            payment.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Fact]
        public void Update_WhenCalled_DoesNotChangeOtherProperties()
        {
            // Arrange
            var payment = CreatePayment();

            // Act
            payment.Update("New", 999m);

            // Assert
            payment.FlatId.Should().Be(_flatId);
            payment.CreatedById.Should().Be(_createdById);
        }
    }
}
