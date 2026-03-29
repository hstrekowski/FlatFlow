using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Chore.Commands.CompleteChoreAssignment;
using FlatFlow.Domain.Enums;
using FlatFlow.Domain.Exceptions;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlatFlow.Application.UnitTests.Features.Chore.Commands;

public class CompleteChoreAssignmentCommandHandlerTests
{
    private readonly Mock<IChoreRepository> _choreRepositoryMock;
    private readonly CompleteChoreAssignmentCommandHandler _handler;

    public CompleteChoreAssignmentCommandHandlerTests()
    {
        _choreRepositoryMock = new Mock<IChoreRepository>();
        _handler = new CompleteChoreAssignmentCommandHandler(
            _choreRepositoryMock.Object,
            Mock.Of<ILogger<CompleteChoreAssignmentCommandHandler>>());
    }

    [Fact]
    public async Task Handle_ExistingActiveAssignment_ShouldCompleteAndReturnUnit()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var chore = flat.AddChore("Sprzątanie", "Opis", ChoreFrequency.Weekly);
        var assignment = chore.AddAssignment(Guid.NewGuid(), DateTime.UtcNow.AddDays(7));
        _choreRepositoryMock
            .Setup(r => r.GetByIdWithAssignmentsAsync(chore.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(chore);

        var command = new CompleteChoreAssignmentCommand(chore.Id, assignment.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        assignment.IsCompleted.Should().BeTrue();
        assignment.CompletedAt.Should().NotBeNull();
        _choreRepositoryMock.Verify(r => r.UpdateAsync(chore, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_AlreadyCompletedAssignment_ShouldThrowDomainException()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var chore = flat.AddChore("Sprzątanie", "Opis", ChoreFrequency.Weekly);
        var assignment = chore.AddAssignment(Guid.NewGuid(), DateTime.UtcNow.AddDays(7));
        assignment.Complete();
        _choreRepositoryMock
            .Setup(r => r.GetByIdWithAssignmentsAsync(chore.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(chore);

        var command = new CompleteChoreAssignmentCommand(chore.Id, assignment.Id);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task Handle_NonExistentAssignmentInChore_ShouldThrowNotFoundException()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var chore = flat.AddChore("Sprzątanie", "Opis", ChoreFrequency.Weekly);
        _choreRepositoryMock
            .Setup(r => r.GetByIdWithAssignmentsAsync(chore.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(chore);

        var command = new CompleteChoreAssignmentCommand(chore.Id, Guid.NewGuid());

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_NonExistingChore_ShouldThrowNotFoundException()
    {
        // Arrange
        var choreId = Guid.NewGuid();
        _choreRepositoryMock
            .Setup(r => r.GetByIdWithAssignmentsAsync(choreId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Chore?)null);

        var command = new CompleteChoreAssignmentCommand(choreId, Guid.NewGuid());

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
