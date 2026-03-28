using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Tenant.Commands.AddTenant;
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
    public async Task Handle_ValidCommand_ShouldReturnTenantId()
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
        result.Should().NotBeEmpty();
        flat.Tenants.Should().ContainSingle(t => t.Email == "jan@test.com");
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

        var command = new AddTenantCommand(flatId, "Jan", "Kowalski", "jan@test.com", "user-1");

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
