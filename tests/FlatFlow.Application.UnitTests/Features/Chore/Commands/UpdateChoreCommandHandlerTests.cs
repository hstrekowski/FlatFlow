using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Chore.Commands.UpdateChore;
using FlatFlow.Domain.Enums;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlatFlow.Application.UnitTests.Features.Chore.Commands;

public class UpdateChoreCommandHandlerTests
{
    private readonly Mock<IChoreRepository> _choreRepositoryMock;
    private readonly UpdateChoreCommandHandler _handler;

    public UpdateChoreCommandHandlerTests()
    {
        _choreRepositoryMock = new Mock<IChoreRepository>();
        _handler = new UpdateChoreCommandHandler(
            _choreRepositoryMock.Object,
            Mock.Of<ILogger<UpdateChoreCommandHandler>>());
    }

    [Fact]
    public async Task Handle_ExistingChore_ShouldUpdateAndReturnUnit()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var chore = flat.AddChore("Old Title", "Old Description", ChoreFrequency.Weekly);
        _choreRepositoryMock
            .Setup(r => r.GetByIdAsync(chore.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(chore);

        var command = new UpdateChoreCommand(chore.Id, "New Title", "New Description", ChoreFrequency.Daily);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        chore.Title.Should().Be("New Title");
        chore.Description.Should().Be("New Description");
        chore.Frequency.Should().Be(ChoreFrequency.Daily);
        _choreRepositoryMock.Verify(r => r.UpdateAsync(chore, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingChore_ShouldThrowNotFoundException()
    {
        // Arrange
        var choreId = Guid.NewGuid();
        _choreRepositoryMock
            .Setup(r => r.GetByIdAsync(choreId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Chore?)null);

        var command = new UpdateChoreCommand(choreId, "Title", "Desc", ChoreFrequency.Weekly);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
