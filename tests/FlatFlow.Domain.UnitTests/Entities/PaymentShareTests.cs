using FlatFlow.Domain.Entities;
using FlatFlow.Domain.Enums;
using FlatFlow.Domain.Exceptions;
using FluentAssertions;

namespace FlatFlow.Domain.UnitTests.Entities
{
    public class PaymentShareTests
    {
        private readonly Guid _tenantId = Guid.NewGuid();
        private readonly Guid _paymentId = Guid.NewGuid();

        private PaymentShare CreatePaymentShare()
            => new(_tenantId, _paymentId, 75.50m);

        [Fact]
        public void Constructor_WithTenantId_SetsTenantId()
        {
            // Arrange & Act
            var share = CreatePaymentShare();

            // Assert
            share.TenantId.Should().Be(_tenantId);
        }

        [Fact]
        public void Constructor_WithPaymentId_SetsPaymentId()
        {
            // Arrange & Act
            var share = CreatePaymentShare();

            // Assert
            share.PaymentId.Should().Be(_paymentId);
        }

        [Theory]
        [InlineData(50.25)]
        [InlineData(1000)]
        public void Constructor_WithValidShareAmount_SetsShareAmount(decimal amount)
        {
            // Arrange & Act
            var share = new PaymentShare(_tenantId, _paymentId, amount);

            // Assert
            share.ShareAmount.Should().Be(amount);
        }

        [Fact]
        public void Constructor_WhenCalled_SetsStatusToNew()
        {
            // Arrange & Act
            var share = CreatePaymentShare();

            // Assert
            share.Status.Should().Be(PaymentShareStatus.New);
        }

        [Fact]
        public void Constructor_WhenCalled_GeneratesNonEmptyId()
        {
            // Arrange & Act
            var share = CreatePaymentShare();

            // Assert
            share.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void Constructor_WhenCalled_SetsCreatedAtToUtcNow()
        {
            // Arrange
            var before = DateTime.UtcNow;

            // Act
            var share = CreatePaymentShare();
            var after = DateTime.UtcNow;

            // Assert
            share.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Fact]
        public void Constructor_WhenCalled_UpdatedAtIsNull()
        {
            // Arrange & Act
            var share = CreatePaymentShare();

            // Assert
            share.UpdatedAt.Should().BeNull();
        }

        [Fact]
        public void Constructor_WhenCalledTwice_GeneratesUniqueIds()
        {
            // Arrange & Act
            var share1 = CreatePaymentShare();
            var share2 = CreatePaymentShare();

            // Assert
            share1.Id.Should().NotBe(share2.Id);
        }

        [Fact]
        public void MarkAsPartial_WhenCalled_SetsStatusToPartial()
        {
            // Arrange
            var share = CreatePaymentShare();

            // Act
            share.MarkAsPartial();

            // Assert
            share.Status.Should().Be(PaymentShareStatus.Partial);
        }

        [Fact]
        public void MarkAsPaid_WhenCalled_SetsStatusToPaid()
        {
            // Arrange
            var share = CreatePaymentShare();

            // Act
            share.MarkAsPaid();

            // Assert
            share.Status.Should().Be(PaymentShareStatus.Paid);
        }

        [Fact]
        public void MarkAsPartial_WhenCalled_SetsUpdatedAt()
        {
            // Arrange
            var share = CreatePaymentShare();
            var before = DateTime.UtcNow;

            // Act
            share.MarkAsPartial();
            var after = DateTime.UtcNow;

            // Assert
            share.UpdatedAt.Should().NotBeNull();
            share.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Fact]
        public void MarkAsPaid_WhenCalled_SetsUpdatedAt()
        {
            // Arrange
            var share = CreatePaymentShare();
            var before = DateTime.UtcNow;

            // Act
            share.MarkAsPaid();
            var after = DateTime.UtcNow;

            // Assert
            share.UpdatedAt.Should().NotBeNull();
            share.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Fact]
        public void MarkAsPaid_WhenCalled_DoesNotChangeOtherProperties()
        {
            // Arrange
            var share = CreatePaymentShare();

            // Act
            share.MarkAsPaid();

            // Assert
            share.TenantId.Should().Be(_tenantId);
            share.PaymentId.Should().Be(_paymentId);
            share.ShareAmount.Should().Be(75.50m);
        }

        [Fact]
        public void MarkAsPartial_WhenCalled_DoesNotChangeOtherProperties()
        {
            // Arrange
            var share = CreatePaymentShare();

            // Act
            share.MarkAsPartial();

            // Assert
            share.TenantId.Should().Be(_tenantId);
            share.PaymentId.Should().Be(_paymentId);
            share.ShareAmount.Should().Be(75.50m);
        }

        [Fact]
        public void MarkAsPartial_WhenAlreadyPaid_ThrowsDomainException()
        {
            // Arrange
            var share = CreatePaymentShare();
            share.MarkAsPaid();

            // Act
            var act = () => share.MarkAsPartial();

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Cannot mark a paid share as partial.");
        }

        [Fact]
        public void MarkAsPaid_WhenAlreadyPaid_ThrowsDomainException()
        {
            // Arrange
            var share = CreatePaymentShare();
            share.MarkAsPaid();

            // Act
            var act = () => share.MarkAsPaid();

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Payment share is already paid.");
        }

        [Fact]
        public void MarkAsPaid_WhenPartial_SetsStatusToPaid()
        {
            // Arrange
            var share = CreatePaymentShare();
            share.MarkAsPartial();

            // Act
            share.MarkAsPaid();

            // Assert
            share.Status.Should().Be(PaymentShareStatus.Paid);
        }

        [Fact]
        public void Constructor_WithEmptyTenantId_ThrowsDomainValidationException()
        {
            // Arrange & Act
            var act = () => new PaymentShare(Guid.Empty, _paymentId, 75.50m);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("Tenant ID cannot be empty.");
        }

        [Fact]
        public void Constructor_WithEmptyPaymentId_ThrowsDomainValidationException()
        {
            // Arrange & Act
            var act = () => new PaymentShare(_tenantId, Guid.Empty, 75.50m);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("Payment ID cannot be empty.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-50.25)]
        public void Constructor_WithInvalidShareAmount_ThrowsDomainValidationException(decimal amount)
        {
            // Arrange & Act
            var act = () => new PaymentShare(_tenantId, _paymentId, amount);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("Share amount must be greater than zero.");
        }
    }
}
