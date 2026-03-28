using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Tenant.Commands.UpdateTenantEmail;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlatFlow.Application.UnitTests.Features.Tenant.Commands;

public class UpdateTenantEmailCommandHandlerTests
{
    private readonly Mock<ITenantRepository> _tenantRepositoryMock;
    private readonly UpdateTenantEmailCommandHandler _handler;

    public UpdateTenantEmailCommandHandlerTests()
    {
        _tenantRepositoryMock = new Mock<ITenantRepository>();
        _handler = new UpdateTenantEmailCommandHandler(
            _tenantRepositoryMock.Object,
            Mock.Of<ILogger<UpdateTenantEmailCommandHandler>>());
    }

    [Fact]
    public async Task Handle_ExistingTenant_ShouldUpdateAndReturnUnit()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var tenant = flat.AddTenant("Jan", "Kowalski", "jan@test.com", "user-1");
        _tenantRepositoryMock
            .Setup(r => r.GetByIdAsync(tenant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenant);

        var command = new UpdateTenantEmailCommand(tenant.Id, "nowy@test.com");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        tenant.Email.Should().Be("nowy@test.com");
        _tenantRepositoryMock.Verify(r => r.UpdateAsync(tenant, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingTenant_ShouldThrowNotFoundException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        _tenantRepositoryMock
            .Setup(r => r.GetByIdAsync(tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Tenant?)null);

        var command = new UpdateTenantEmailCommand(tenantId, "nowy@test.com");

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
