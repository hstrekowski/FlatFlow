using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Chore.Commands.AddChoreAssignment;
using FlatFlow.Domain.Enums;
using FlatFlow.Domain.Exceptions;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlatFlow.Application.UnitTests.Features.Chore.Commands;

public class AddChoreAssignmentCommandHandlerTests
{
    private readonly Mock<IChoreRepository> _choreRepositoryMock;
    private readonly AddChoreAssignmentCommandHandler _handler;

    public AddChoreAssignmentCommandHandlerTests()
    {
        _choreRepositoryMock = new Mock<IChoreRepository>();
        _handler = new AddChoreAssignmentCommandHandler(
            _choreRepositoryMock.Object,
            Mock.Of<ILogger<AddChoreAssignmentCommandHandler>>());
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldReturnAssignmentIdAndAddToChore()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var chore = flat.AddChore("Sprzątanie", "Opis", ChoreFrequency.Weekly);
        var tenantId = Guid.NewGuid();
        var dueDate = DateTime.UtcNow.AddDays(7);
        _choreRepositoryMock
            .Setup(r => r.GetByIdWithAssignmentsAsync(chore.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(chore);

        var command = new AddChoreAssignmentCommand(chore.Id, tenantId, dueDate);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        var addedAssignment = chore.ChoreAssignments.Should().ContainSingle().Subject;
        result.Should().Be(addedAssignment.Id);
        addedAssignment.TenantId.Should().Be(tenantId);
        addedAssignment.DueDate.Should().Be(dueDate);
        _choreRepositoryMock.Verify(r => r.UpdateAsync(chore, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateActiveTenantAssignment_ShouldThrowDomainException()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var chore = flat.AddChore("Sprzątanie", "Opis", ChoreFrequency.Weekly);
        var tenantId = Guid.NewGuid();
        chore.AddAssignment(tenantId, DateTime.UtcNow.AddDays(7));
        _choreRepositoryMock
            .Setup(r => r.GetByIdWithAssignmentsAsync(chore.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(chore);

        var command = new AddChoreAssignmentCommand(chore.Id, tenantId, DateTime.UtcNow.AddDays(14));

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

        var command = new AddChoreAssignmentCommand(choreId, Guid.NewGuid(), DateTime.UtcNow.AddDays(7));

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
