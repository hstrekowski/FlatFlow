using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Identity;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Chore.Commands.RemoveChore;
using FlatFlow.Domain.Enums;
using FlatFlow.Domain.Exceptions;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlatFlow.Application.UnitTests.Features.Chore.Commands;

public class RemoveChoreCommandHandlerTests
{
    private const string TestUserId = "test-user-id";
    private readonly Mock<IFlatRepository> _flatRepositoryMock;
    private readonly Mock<ITenantRepository> _tenantRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly RemoveChoreCommandHandler _handler;

    public RemoveChoreCommandHandlerTests()
    {
        _flatRepositoryMock = new Mock<IFlatRepository>();
        _tenantRepositoryMock = new Mock<ITenantRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _currentUserServiceMock.Setup(s => s.UserId).Returns(TestUserId);
        _tenantRepositoryMock
            .Setup(r => r.GetByUserIdAndFlatIdAsync(TestUserId, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Domain.Entities.Tenant("Owner", "Test", "owner@test.com", TestUserId, Guid.NewGuid(), isOwner: true));
        _handler = new RemoveChoreCommandHandler(
            _flatRepositoryMock.Object,
            _tenantRepositoryMock.Object,
            _currentUserServiceMock.Object,
            Mock.Of<ILogger<RemoveChoreCommandHandler>>());
    }

    [Fact]
    public async Task Handle_ExistingFlatAndChore_ShouldRemoveAndReturnUnit()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var chore = flat.AddChore("Sprzątanie", "Opis", ChoreFrequency.Weekly, Guid.NewGuid());
        _flatRepositoryMock
            .Setup(r => r.GetByIdWithChoresAsync(flat.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(flat);

        var command = new RemoveChoreCommand(flat.Id, chore.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        flat.Chores.Should().BeEmpty();
        _flatRepositoryMock.Verify(r => r.UpdateAsync(flat, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistentChoreInFlat_ShouldThrowNotFoundException()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        _flatRepositoryMock
            .Setup(r => r.GetByIdWithChoresAsync(flat.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(flat);

        var command = new RemoveChoreCommand(flat.Id, Guid.NewGuid());

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_NonExistingFlat_ShouldThrowNotFoundException()
    {
        // Arrange
        var flatId = Guid.NewGuid();
        _flatRepositoryMock
            .Setup(r => r.GetByIdWithChoresAsync(flatId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Flat?)null);

        var command = new RemoveChoreCommand(flatId, Guid.NewGuid());

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_OwnerRemovesOthersChore_ShouldSucceed()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var otherTenantId = Guid.NewGuid();
        var chore = flat.AddChore("Sprzątanie", "Opis", ChoreFrequency.Weekly, otherTenantId);
        var ownerTenant = new Domain.Entities.Tenant("Owner", "Test", "owner@test.com", TestUserId, flat.Id, isOwner: true);
        _flatRepositoryMock
            .Setup(r => r.GetByIdWithChoresAsync(flat.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(flat);
        _tenantRepositoryMock
            .Setup(r => r.GetByUserIdAndFlatIdAsync(TestUserId, flat.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ownerTenant);

        var command = new RemoveChoreCommand(flat.Id, chore.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        flat.Chores.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_MemberRemovesOwnChore_ShouldSucceed()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var memberTenant = new Domain.Entities.Tenant("Member", "Test", "member@test.com", TestUserId, flat.Id, isOwner: false);
        var chore = flat.AddChore("Sprzątanie", "Opis", ChoreFrequency.Weekly, memberTenant.Id);
        _flatRepositoryMock
            .Setup(r => r.GetByIdWithChoresAsync(flat.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(flat);
        _tenantRepositoryMock
            .Setup(r => r.GetByUserIdAndFlatIdAsync(TestUserId, flat.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(memberTenant);

        var command = new RemoveChoreCommand(flat.Id, chore.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        flat.Chores.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_MemberRemovesOthersChore_ShouldThrowForbiddenException()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var otherTenantId = Guid.NewGuid();
        var chore = flat.AddChore("Sprzątanie", "Opis", ChoreFrequency.Weekly, otherTenantId);
        var memberTenant = new Domain.Entities.Tenant("Member", "Test", "member@test.com", TestUserId, flat.Id, isOwner: false);
        _flatRepositoryMock
            .Setup(r => r.GetByIdWithChoresAsync(flat.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(flat);
        _tenantRepositoryMock
            .Setup(r => r.GetByUserIdAndFlatIdAsync(TestUserId, flat.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(memberTenant);

        var command = new RemoveChoreCommand(flat.Id, chore.Id);

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
        _flatRepositoryMock
            .Setup(r => r.GetByIdWithChoresAsync(flat.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(flat);
        _tenantRepositoryMock
            .Setup(r => r.GetByUserIdAndFlatIdAsync(TestUserId, flat.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Tenant?)null);

        var command = new RemoveChoreCommand(flat.Id, chore.Id);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }
}
