using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Payment.Commands.RemovePaymentShare;
using FlatFlow.Domain.Exceptions;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlatFlow.Application.UnitTests.Features.Payment.Commands;

public class RemovePaymentShareCommandHandlerTests
{
    private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
    private readonly RemovePaymentShareCommandHandler _handler;

    public RemovePaymentShareCommandHandlerTests()
    {
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        _handler = new RemovePaymentShareCommandHandler(
            _paymentRepositoryMock.Object,
            Mock.Of<ILogger<RemovePaymentShareCommandHandler>>());
    }

    [Fact]
    public async Task Handle_ExistingShare_ShouldRemoveAndReturnUnit()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var payment = flat.AddPayment("Czynsz", 1500m, DateTime.UtcNow, Guid.NewGuid());
        var share = payment.AddShare(Guid.NewGuid(), 500m);
        _paymentRepositoryMock
            .Setup(r => r.GetByIdWithSharesAsync(payment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);

        var command = new RemovePaymentShareCommand(payment.Id, share.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        payment.PaymentShares.Should().BeEmpty();
        _paymentRepositoryMock.Verify(r => r.UpdateAsync(payment, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistentShare_ShouldThrowDomainException()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var payment = flat.AddPayment("Czynsz", 1500m, DateTime.UtcNow, Guid.NewGuid());
        _paymentRepositoryMock
            .Setup(r => r.GetByIdWithSharesAsync(payment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);

        var command = new RemovePaymentShareCommand(payment.Id, Guid.NewGuid());

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task Handle_NonExistingPayment_ShouldThrowNotFoundException()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        _paymentRepositoryMock
            .Setup(r => r.GetByIdWithSharesAsync(paymentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Payment?)null);

        var command = new RemovePaymentShareCommand(paymentId, Guid.NewGuid());

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
