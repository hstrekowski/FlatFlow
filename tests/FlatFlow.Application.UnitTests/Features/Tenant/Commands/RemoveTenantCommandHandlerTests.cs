using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Tenant.Commands.RemoveTenant;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlatFlow.Application.UnitTests.Features.Tenant.Commands;

public class RemoveTenantCommandHandlerTests
{
    private readonly Mock<IFlatRepository> _flatRepositoryMock;
    private readonly RemoveTenantCommandHandler _handler;

    public RemoveTenantCommandHandlerTests()
    {
        _flatRepositoryMock = new Mock<IFlatRepository>();
        _handler = new RemoveTenantCommandHandler(
            _flatRepositoryMock.Object,
            Mock.Of<ILogger<RemoveTenantCommandHandler>>());
    }

    [Fact]
    public async Task Handle_ExistingFlatAndTenant_ShouldRemoveAndReturnUnit()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var tenant = flat.AddTenant("Jan", "Kowalski", "jan@test.com", "user-1");
        _flatRepositoryMock
            .Setup(r => r.GetByIdWithTenantsAsync(flat.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(flat);

        var command = new RemoveTenantCommand(flat.Id, tenant.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        flat.Tenants.Should().BeEmpty();
        _flatRepositoryMock.Verify(r => r.UpdateAsync(flat, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingFlat_ShouldThrowNotFoundException()
    {
        // Arrange
        var flatId = Guid.NewGuid();
        _flatRepositoryMock
            .Setup(r => r.GetByIdWithTenantsAsync(flatId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Flat?)null);

        var command = new RemoveTenantCommand(flatId, Guid.NewGuid());

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
