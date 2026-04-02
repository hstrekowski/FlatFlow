using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Identity;
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
    private const string TestUserId = "test-user-id";
    private readonly Mock<IChoreRepository> _choreRepositoryMock;
    private readonly Mock<ITenantRepository> _tenantRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly UpdateChoreCommandHandler _handler;

    public UpdateChoreCommandHandlerTests()
    {
        _choreRepositoryMock = new Mock<IChoreRepository>();
        _tenantRepositoryMock = new Mock<ITenantRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _currentUserServiceMock.Setup(s => s.UserId).Returns(TestUserId);
        _tenantRepositoryMock
            .Setup(r => r.GetByUserIdAndFlatIdAsync(TestUserId, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Domain.Entities.Tenant("Owner", "Test", "owner@test.com", TestUserId, Guid.NewGuid(), isOwner: true));
        _handler = new UpdateChoreCommandHandler(
            _choreRepositoryMock.Object,
            _tenantRepositoryMock.Object,
            _currentUserServiceMock.Object,
            Mock.Of<ILogger<UpdateChoreCommandHandler>>());
    }

    [Fact]
    public async Task Handle_ExistingChore_ShouldUpdateAndReturnUnit()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var chore = flat.AddChore("Old Title", "Old Description", ChoreFrequency.Weekly, Guid.NewGuid());
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

    [Fact]
    public async Task Handle_OwnerModifiesOthersChore_ShouldSucceed()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var otherTenantId = Guid.NewGuid();
        var chore = flat.AddChore("Title", "Desc", ChoreFrequency.Weekly, otherTenantId);
        var ownerTenant = new Domain.Entities.Tenant("Owner", "Test", "owner@test.com", TestUserId, flat.Id, isOwner: true);
        _choreRepositoryMock
            .Setup(r => r.GetByIdAsync(chore.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(chore);
        _tenantRepositoryMock
            .Setup(r => r.GetByUserIdAndFlatIdAsync(TestUserId, flat.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ownerTenant);

        var command = new UpdateChoreCommand(chore.Id, "Updated", "Updated", ChoreFrequency.Daily);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        chore.Title.Should().Be("Updated");
    }

    [Fact]
    public async Task Handle_MemberModifiesOwnChore_ShouldSucceed()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var memberTenant = new Domain.Entities.Tenant("Member", "Test", "member@test.com", TestUserId, flat.Id, isOwner: false);
        var chore = flat.AddChore("Title", "Desc", ChoreFrequency.Weekly, memberTenant.Id);
        _choreRepositoryMock
            .Setup(r => r.GetByIdAsync(chore.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(chore);
        _tenantRepositoryMock
            .Setup(r => r.GetByUserIdAndFlatIdAsync(TestUserId, flat.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(memberTenant);

        var command = new UpdateChoreCommand(chore.Id, "Updated", "Updated", ChoreFrequency.Daily);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        chore.Title.Should().Be("Updated");
    }

    [Fact]
    public async Task Handle_MemberModifiesOthersChore_ShouldThrowForbiddenException()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var otherTenantId = Guid.NewGuid();
        var chore = flat.AddChore("Title", "Desc", ChoreFrequency.Weekly, otherTenantId);
        var memberTenant = new Domain.Entities.Tenant("Member", "Test", "member@test.com", TestUserId, flat.Id, isOwner: false);
        _choreRepositoryMock
            .Setup(r => r.GetByIdAsync(chore.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(chore);
        _tenantRepositoryMock
            .Setup(r => r.GetByUserIdAndFlatIdAsync(TestUserId, flat.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(memberTenant);

        var command = new UpdateChoreCommand(chore.Id, "Updated", "Updated", ChoreFrequency.Daily);

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
        var chore = flat.AddChore("Title", "Desc", ChoreFrequency.Weekly, Guid.NewGuid());
        _choreRepositoryMock
            .Setup(r => r.GetByIdAsync(chore.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(chore);
        _tenantRepositoryMock
            .Setup(r => r.GetByUserIdAndFlatIdAsync(TestUserId, flat.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Tenant?)null);

        var command = new UpdateChoreCommand(chore.Id, "Updated", "Updated", ChoreFrequency.Daily);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }
}
