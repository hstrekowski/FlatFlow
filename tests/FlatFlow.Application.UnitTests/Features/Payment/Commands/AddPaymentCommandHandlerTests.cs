using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Payment.Commands.AddPayment;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlatFlow.Application.UnitTests.Features.Payment.Commands;

public class AddPaymentCommandHandlerTests
{
    private readonly Mock<IFlatRepository> _flatRepositoryMock;
    private readonly AddPaymentCommandHandler _handler;

    public AddPaymentCommandHandlerTests()
    {
        _flatRepositoryMock = new Mock<IFlatRepository>();
        _handler = new AddPaymentCommandHandler(
            _flatRepositoryMock.Object,
            Mock.Of<ILogger<AddPaymentCommandHandler>>());
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldReturnPaymentIdAndAddToFlat()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var createdById = Guid.NewGuid();
        var dueDate = DateTime.UtcNow.AddDays(30);
        _flatRepositoryMock
            .Setup(r => r.GetByIdWithPaymentsAsync(flat.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(flat);

        var command = new AddPaymentCommand(flat.Id, "Czynsz", 1500m, dueDate, createdById);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        var addedPayment = flat.Payments.Should().ContainSingle().Subject;
        result.Should().Be(addedPayment.Id);
        addedPayment.Title.Should().Be("Czynsz");
        addedPayment.Amount.Should().Be(1500m);
        addedPayment.DueDate.Should().Be(dueDate);
        addedPayment.CreatedById.Should().Be(createdById);
        _flatRepositoryMock.Verify(r => r.UpdateAsync(flat, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingFlat_ShouldThrowNotFoundException()
    {
        // Arrange
        var flatId = Guid.NewGuid();
        _flatRepositoryMock
            .Setup(r => r.GetByIdWithPaymentsAsync(flatId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Flat?)null);

        var command = new AddPaymentCommand(flatId, "Czynsz", 1500m, DateTime.UtcNow, Guid.NewGuid());

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
