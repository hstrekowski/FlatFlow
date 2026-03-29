using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Chore.Commands.RemoveChoreAssignment;
using FlatFlow.Domain.Enums;
using FlatFlow.Domain.Exceptions;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlatFlow.Application.UnitTests.Features.Chore.Commands;

public class RemoveChoreAssignmentCommandHandlerTests
{
    private readonly Mock<IChoreRepository> _choreRepositoryMock;
    private readonly RemoveChoreAssignmentCommandHandler _handler;

    public RemoveChoreAssignmentCommandHandlerTests()
    {
        _choreRepositoryMock = new Mock<IChoreRepository>();
        _handler = new RemoveChoreAssignmentCommandHandler(
            _choreRepositoryMock.Object,
            Mock.Of<ILogger<RemoveChoreAssignmentCommandHandler>>());
    }

    [Fact]
    public async Task Handle_ExistingAssignment_ShouldRemoveAndReturnUnit()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var chore = flat.AddChore("Sprzątanie", "Opis", ChoreFrequency.Weekly);
        var assignment = chore.AddAssignment(Guid.NewGuid(), DateTime.UtcNow.AddDays(7));
        _choreRepositoryMock
            .Setup(r => r.GetByIdWithAssignmentsAsync(chore.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(chore);

        var command = new RemoveChoreAssignmentCommand(chore.Id, assignment.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        chore.ChoreAssignments.Should().BeEmpty();
        _choreRepositoryMock.Verify(r => r.UpdateAsync(chore, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistentAssignment_ShouldThrowDomainException()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var chore = flat.AddChore("Sprzątanie", "Opis", ChoreFrequency.Weekly);
        _choreRepositoryMock
            .Setup(r => r.GetByIdWithAssignmentsAsync(chore.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(chore);

        var command = new RemoveChoreAssignmentCommand(chore.Id, Guid.NewGuid());

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task Handle_NonExistingChore_ShouldThrowNotFoundException()
    {
        // Arrange
        var choreId = Guid.NewGuid();
        _choreRepositoryMock
            .Setup(r => r.GetByIdWithAssignmentsAsync(choreId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Chore?)null);

        var command = new RemoveChoreAssignmentCommand(choreId, Guid.NewGuid());

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
