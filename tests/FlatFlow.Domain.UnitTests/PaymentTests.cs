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
            var payment = new Payment(title, 100m, _flatId, _createdById);

            payment.Title.Should().Be(title);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(99.99)]
        [InlineData(1500.00)]
        public void Constructor_WithValidAmount_SetsAmount(decimal amount)
        {
            var payment = new Payment("Title", amount, _flatId, _createdById);

            payment.Amount.Should().Be(amount);
        }

        [Fact]
        public void Constructor_WithFlatId_SetsFlatId()
        {
            var payment = CreatePayment();

            payment.FlatId.Should().Be(_flatId);
        }

        [Fact]
        public void Constructor_WithCreatedById_SetsCreatedById()
        {
            var payment = CreatePayment();

            payment.CreatedById.Should().Be(_createdById);
        }

        [Fact]
        public void Constructor_WhenCalled_GeneratesNonEmptyId()
        {
            var payment = CreatePayment();

            payment.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void Constructor_WhenCalled_SetsCreatedAtToUtcNow()
        {
            var before = DateTime.UtcNow;
            var payment = CreatePayment();
            var after = DateTime.UtcNow;

            payment.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Fact]
        public void Constructor_WhenCalled_InitializesEmptyPaymentShares()
        {
            var payment = CreatePayment();

            payment.PaymentShares.Should().BeEmpty();
        }

        [Fact]
        public void Constructor_WhenCalledTwice_GeneratesUniqueIds()
        {
            var payment1 = CreatePayment();
            var payment2 = CreatePayment();

            payment1.Id.Should().NotBe(payment2.Id);
        }

        [Theory]
        [InlineData("New Title", 200.00)]
        [InlineData("Updated", 0)]
        public void Update_WithNewValues_ChangesTitleAndAmount(string title, decimal amount)
        {
            var payment = CreatePayment();

            payment.Update(title, amount);

            payment.Title.Should().Be(title);
            payment.Amount.Should().Be(amount);
        }

        [Fact]
        public void Update_WhenCalled_DoesNotChangeOtherProperties()
        {
            var payment = CreatePayment();

            payment.Update("New", 999m);

            payment.FlatId.Should().Be(_flatId);
            payment.CreatedById.Should().Be(_createdById);
        }
    }
}
