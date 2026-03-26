using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Flat.Commands.RefreshAccessCode;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlatFlow.Application.UnitTests.Features.Flat.Commands;

public class RefreshAccessCodeCommandHandlerTests
{
    private readonly Mock<IFlatRepository> _flatRepositoryMock;
    private readonly RefreshAccessCodeCommandHandler _handler;

    public RefreshAccessCodeCommandHandlerTests()
    {
        _flatRepositoryMock = new Mock<IFlatRepository>();
        _handler = new RefreshAccessCodeCommandHandler(
            _flatRepositoryMock.Object,
            Mock.Of<ILogger<RefreshAccessCodeCommandHandler>>());
    }

    [Fact]
    public async Task Handle_ExistingFlat_ShouldRefreshAndReturnUnit()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Test", new Address("St", "City", "00-000", "Country"));
        var originalCode = flat.AccessCode;
        _flatRepositoryMock
            .Setup(r => r.GetByIdAsync(flat.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(flat);

        var command = new RefreshAccessCodeCommand(flat.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        flat.AccessCode.Should().NotBe(originalCode);
        _flatRepositoryMock.Verify(r => r.UpdateAsync(flat, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingFlat_ShouldThrowNotFoundException()
    {
        // Arrange
        var flatId = Guid.NewGuid();
        _flatRepositoryMock
            .Setup(r => r.GetByIdAsync(flatId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Flat?)null);

        var command = new RefreshAccessCodeCommand(flatId);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
