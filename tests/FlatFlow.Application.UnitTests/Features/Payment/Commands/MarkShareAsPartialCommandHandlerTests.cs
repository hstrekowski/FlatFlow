using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Identity;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Payment.Commands.MarkShareAsPartial;
using FlatFlow.Domain.Enums;
using FlatFlow.Domain.Exceptions;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlatFlow.Application.UnitTests.Features.Payment.Commands;

public class MarkShareAsPartialCommandHandlerTests
{
    private const string TestUserId = "test-user-id";
    private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
    private readonly Mock<ITenantRepository> _tenantRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly MarkShareAsPartialCommandHandler _handler;

    public MarkShareAsPartialCommandHandlerTests()
    {
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        _tenantRepositoryMock = new Mock<ITenantRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _currentUserServiceMock.Setup(s => s.UserId).Returns(TestUserId);
        _tenantRepositoryMock
            .Setup(r => r.GetByUserIdAndFlatIdAsync(TestUserId, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Domain.Entities.Tenant("Owner", "Test", "owner@test.com", TestUserId, Guid.NewGuid(), isOwner: true));
        _handler = new MarkShareAsPartialCommandHandler(
            _paymentRepositoryMock.Object,
            _tenantRepositoryMock.Object,
            _currentUserServiceMock.Object,
            Mock.Of<ILogger<MarkShareAsPartialCommandHandler>>());
    }

    [Fact]
    public async Task Handle_NewShare_ShouldMarkAsPartialAndReturnUnit()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var payment = flat.AddPayment("Czynsz", 1500m, DateTime.UtcNow, Guid.NewGuid());
        var share = payment.AddShare(Guid.NewGuid(), 500m);
        _paymentRepositoryMock
            .Setup(r => r.GetByIdWithSharesAsync(payment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);

        var command = new MarkShareAsPartialCommand(payment.Id, share.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        share.Status.Should().Be(PaymentShareStatus.Partial);
        _paymentRepositoryMock.Verify(r => r.UpdateAsync(payment, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_AlreadyPaidShare_ShouldThrowDomainException()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var payment = flat.AddPayment("Czynsz", 1500m, DateTime.UtcNow, Guid.NewGuid());
        var share = payment.AddShare(Guid.NewGuid(), 500m);
        share.MarkAsPaid();
        _paymentRepositoryMock
            .Setup(r => r.GetByIdWithSharesAsync(payment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);

        var command = new MarkShareAsPartialCommand(payment.Id, share.Id);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task Handle_NonExistentShareInPayment_ShouldThrowNotFoundException()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var payment = flat.AddPayment("Czynsz", 1500m, DateTime.UtcNow, Guid.NewGuid());
        _paymentRepositoryMock
            .Setup(r => r.GetByIdWithSharesAsync(payment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);

        var command = new MarkShareAsPartialCommand(payment.Id, Guid.NewGuid());

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_NonExistingPayment_ShouldThrowNotFoundException()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        _paymentRepositoryMock
            .Setup(r => r.GetByIdWithSharesAsync(paymentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Payment?)null);

        var command = new MarkShareAsPartialCommand(paymentId, Guid.NewGuid());

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
