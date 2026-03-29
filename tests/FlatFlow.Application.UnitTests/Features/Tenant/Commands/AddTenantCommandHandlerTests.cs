using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Tenant.Commands.AddTenant;
using FlatFlow.Domain.Exceptions;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlatFlow.Application.UnitTests.Features.Tenant.Commands;

public class AddTenantCommandHandlerTests
{
    private readonly Mock<IFlatRepository> _flatRepositoryMock;
    private readonly AddTenantCommandHandler _handler;

    public AddTenantCommandHandlerTests()
    {
        _flatRepositoryMock = new Mock<IFlatRepository>();
        _handler = new AddTenantCommandHandler(
            _flatRepositoryMock.Object,
            Mock.Of<ILogger<AddTenantCommandHandler>>());
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldReturnTenantIdAndAddToFlat()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        _flatRepositoryMock
            .Setup(r => r.GetByIdWithTenantsAsync(flat.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(flat);

        var command = new AddTenantCommand(flat.Id, "Jan", "Kowalski", "jan@test.com", "user-1");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        var addedTenant = flat.Tenants.Should().ContainSingle().Subject;
        result.Should().Be(addedTenant.Id);
        addedTenant.FirstName.Should().Be("Jan");
        addedTenant.LastName.Should().Be("Kowalski");
        addedTenant.Email.Should().Be("jan@test.com");
        addedTenant.IsOwner.Should().BeFalse();
        _flatRepositoryMock.Verify(r => r.UpdateAsync(flat, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithIsOwnerTrue_ShouldCreateOwnerTenant()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        _flatRepositoryMock
            .Setup(r => r.GetByIdWithTenantsAsync(flat.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(flat);

        var command = new AddTenantCommand(flat.Id, "Jan", "Kowalski", "jan@test.com", "user-1", IsOwner: true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        var addedTenant = flat.Tenants.Should().ContainSingle().Subject;
        result.Should().Be(addedTenant.Id);
        addedTenant.IsOwner.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_DuplicateUserId_ShouldThrowDomainException()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        flat.AddTenant("Jan", "Kowalski", "jan@test.com", "user-1");
        _flatRepositoryMock
            .Setup(r => r.GetByIdWithTenantsAsync(flat.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(flat);

        var command = new AddTenantCommand(flat.Id, "Anna", "Nowak", "anna@test.com", "user-1");

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task Handle_NonExistingFlat_ShouldThrowNotFoundException()
    {
        // Arrange
        var flatId = Guid.NewGuid();
        _flatRepositoryMock
            .Setup(r => r.GetByIdWithTenantsAsync(flatId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Flat?)null);

        var command = new AddTenantCommand(flatId, "Jan", "Kowalski", "jan@test.com", "user-1");

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
