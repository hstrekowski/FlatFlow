using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Tenant.Commands.JoinFlat;
using FlatFlow.Domain.Exceptions;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlatFlow.Application.UnitTests.Features.Tenant.Commands;

public class JoinFlatCommandHandlerTests
{
    private readonly Mock<IFlatRepository> _flatRepositoryMock;
    private readonly JoinFlatCommandHandler _handler;

    public JoinFlatCommandHandlerTests()
    {
        _flatRepositoryMock = new Mock<IFlatRepository>();
        _handler = new JoinFlatCommandHandler(
            _flatRepositoryMock.Object,
            Mock.Of<ILogger<JoinFlatCommandHandler>>());
    }

    [Fact]
    public async Task Handle_ValidAccessCode_ShouldAddTenantAndReturnId()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        _flatRepositoryMock
            .Setup(r => r.GetByAccessCodeWithTenantsAsync(flat.AccessCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(flat);

        var command = new JoinFlatCommand(flat.AccessCode, "Jan", "Kowalski", "jan@test.com", "user-1");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        var addedTenant = flat.Tenants.Should().ContainSingle().Subject;
        result.Should().Be(addedTenant.Id);
        addedTenant.FirstName.Should().Be("Jan");
        addedTenant.IsOwner.Should().BeFalse();
        _flatRepositoryMock.Verify(r => r.UpdateAsync(flat, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateUserId_ShouldThrowDomainException()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        flat.AddTenant("Jan", "Kowalski", "jan@test.com", "user-1");
        _flatRepositoryMock
            .Setup(r => r.GetByAccessCodeWithTenantsAsync(flat.AccessCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(flat);

        var command = new JoinFlatCommand(flat.AccessCode, "Anna", "Nowak", "anna@test.com", "user-1");

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task Handle_InvalidAccessCode_ShouldThrowNotFoundException()
    {
        // Arrange
        _flatRepositoryMock
            .Setup(r => r.GetByAccessCodeWithTenantsAsync("INVALID", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Flat?)null);

        var command = new JoinFlatCommand("INVALID", "Jan", "Kowalski", "jan@test.com", "user-1");

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
