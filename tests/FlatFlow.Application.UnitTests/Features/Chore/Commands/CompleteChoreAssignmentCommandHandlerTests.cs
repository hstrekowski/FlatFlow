using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Identity;
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
    private const string TestUserId = "test-user-id";
    private readonly Mock<IChoreRepository> _choreRepositoryMock;
    private readonly Mock<ITenantRepository> _tenantRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly CompleteChoreAssignmentCommandHandler _handler;

    public CompleteChoreAssignmentCommandHandlerTests()
    {
        _choreRepositoryMock = new Mock<IChoreRepository>();
        _tenantRepositoryMock = new Mock<ITenantRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _currentUserServiceMock.Setup(s => s.UserId).Returns(TestUserId);
        _tenantRepositoryMock
            .Setup(r => r.GetByUserIdAndFlatIdAsync(TestUserId, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Domain.Entities.Tenant("Owner", "Test", "owner@test.com", TestUserId, Guid.NewGuid(), isOwner: true));
        _handler = new CompleteChoreAssignmentCommandHandler(
            _choreRepositoryMock.Object,
            _tenantRepositoryMock.Object,
            _currentUserServiceMock.Object,
            Mock.Of<ILogger<CompleteChoreAssignmentCommandHandler>>());
    }

    [Fact]
    public async Task Handle_ExistingActiveAssignment_ShouldCompleteAndReturnUnit()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var chore = flat.AddChore("Sprzątanie", "Opis", ChoreFrequency.Weekly, Guid.NewGuid());
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
        var chore = flat.AddChore("Sprzątanie", "Opis", ChoreFrequency.Weekly, Guid.NewGuid());
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
        var chore = flat.AddChore("Sprzątanie", "Opis", ChoreFrequency.Weekly, Guid.NewGuid());
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

    [Fact]
    public async Task Handle_OwnerCompletesOthersAssignment_ShouldSucceed()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var chore = flat.AddChore("Sprzątanie", "Opis", ChoreFrequency.Weekly, Guid.NewGuid());
        var otherTenantId = Guid.NewGuid();
        var assignment = chore.AddAssignment(otherTenantId, DateTime.UtcNow.AddDays(7));
        var ownerTenant = new Domain.Entities.Tenant("Owner", "Test", "owner@test.com", TestUserId, flat.Id, isOwner: true);
        _choreRepositoryMock
            .Setup(r => r.GetByIdWithAssignmentsAsync(chore.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(chore);
        _tenantRepositoryMock
            .Setup(r => r.GetByUserIdAndFlatIdAsync(TestUserId, flat.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ownerTenant);

        var command = new CompleteChoreAssignmentCommand(chore.Id, assignment.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        assignment.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_MemberCompletesOwnAssignment_ShouldSucceed()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var chore = flat.AddChore("Sprzątanie", "Opis", ChoreFrequency.Weekly, Guid.NewGuid());
        var memberTenant = new Domain.Entities.Tenant("Member", "Test", "member@test.com", TestUserId, flat.Id, isOwner: false);
        var assignment = chore.AddAssignment(memberTenant.Id, DateTime.UtcNow.AddDays(7));
        _choreRepositoryMock
            .Setup(r => r.GetByIdWithAssignmentsAsync(chore.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(chore);
        _tenantRepositoryMock
            .Setup(r => r.GetByUserIdAndFlatIdAsync(TestUserId, flat.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(memberTenant);

        var command = new CompleteChoreAssignmentCommand(chore.Id, assignment.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        assignment.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_MemberCompletesOthersAssignment_ShouldThrowForbiddenException()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var chore = flat.AddChore("Sprzątanie", "Opis", ChoreFrequency.Weekly, Guid.NewGuid());
        var otherTenantId = Guid.NewGuid();
        var assignment = chore.AddAssignment(otherTenantId, DateTime.UtcNow.AddDays(7));
        var memberTenant = new Domain.Entities.Tenant("Member", "Test", "member@test.com", TestUserId, flat.Id, isOwner: false);
        _choreRepositoryMock
            .Setup(r => r.GetByIdWithAssignmentsAsync(chore.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(chore);
        _tenantRepositoryMock
            .Setup(r => r.GetByUserIdAndFlatIdAsync(TestUserId, flat.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(memberTenant);

        var command = new CompleteChoreAssignmentCommand(chore.Id, assignment.Id);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Handle_UserNotTenantInFlat_ShouldThrowForbiddenException()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var chore = flat.AddChore("Sprzątanie", "Opis", ChoreFrequency.Weekly, Guid.NewGuid());
        var assignment = chore.AddAssignment(Guid.NewGuid(), DateTime.UtcNow.AddDays(7));
        _choreRepositoryMock
            .Setup(r => r.GetByIdWithAssignmentsAsync(chore.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(chore);
        _tenantRepositoryMock
            .Setup(r => r.GetByUserIdAndFlatIdAsync(TestUserId, flat.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Tenant?)null);

        var command = new CompleteChoreAssignmentCommand(chore.Id, assignment.Id);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }
}
