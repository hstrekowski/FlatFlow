using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Chore.Commands.AddChore;
using FlatFlow.Domain.Enums;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlatFlow.Application.UnitTests.Features.Chore.Commands;

public class AddChoreCommandHandlerTests
{
    private readonly Mock<IFlatRepository> _flatRepositoryMock;
    private readonly AddChoreCommandHandler _handler;

    public AddChoreCommandHandlerTests()
    {
        _flatRepositoryMock = new Mock<IFlatRepository>();
        _handler = new AddChoreCommandHandler(
            _flatRepositoryMock.Object,
            Mock.Of<ILogger<AddChoreCommandHandler>>());
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldReturnChoreIdAndAddToFlat()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        _flatRepositoryMock
            .Setup(r => r.GetByIdWithChoresAsync(flat.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(flat);

        var command = new AddChoreCommand(flat.Id, "Sprzątanie", "Posprzątać kuchnię", ChoreFrequency.Weekly);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        var addedChore = flat.Chores.Should().ContainSingle().Subject;
        result.Should().Be(addedChore.Id);
        addedChore.Title.Should().Be("Sprzątanie");
        addedChore.Description.Should().Be("Posprzątać kuchnię");
        addedChore.Frequency.Should().Be(ChoreFrequency.Weekly);
        _flatRepositoryMock.Verify(r => r.UpdateAsync(flat, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingFlat_ShouldThrowNotFoundException()
    {
        // Arrange
        var flatId = Guid.NewGuid();
        _flatRepositoryMock
            .Setup(r => r.GetByIdWithChoresAsync(flatId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Flat?)null);

        var command = new AddChoreCommand(flatId, "Sprzątanie", "Opis", ChoreFrequency.Weekly);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
