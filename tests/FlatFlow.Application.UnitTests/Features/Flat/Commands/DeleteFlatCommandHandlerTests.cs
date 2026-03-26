using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Flat.Commands.DeleteFlat;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlatFlow.Application.UnitTests.Features.Flat.Commands;

public class DeleteFlatCommandHandlerTests
{
    private readonly Mock<IFlatRepository> _flatRepositoryMock;
    private readonly DeleteFlatCommandHandler _handler;

    public DeleteFlatCommandHandlerTests()
    {
        _flatRepositoryMock = new Mock<IFlatRepository>();
        _handler = new DeleteFlatCommandHandler(
            _flatRepositoryMock.Object,
            Mock.Of<ILogger<DeleteFlatCommandHandler>>());
    }

    [Fact]
    public async Task Handle_ExistingFlat_ShouldDeleteAndReturnUnit()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Test", new Address("St", "City", "00-000", "Country"));
        _flatRepositoryMock
            .Setup(r => r.GetByIdAsync(flat.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(flat);

        var command = new DeleteFlatCommand(flat.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        _flatRepositoryMock.Verify(r => r.DeleteAsync(flat, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingFlat_ShouldThrowNotFoundException()
    {
        // Arrange
        var flatId = Guid.NewGuid();
        _flatRepositoryMock
            .Setup(r => r.GetByIdAsync(flatId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Flat?)null);

        var command = new DeleteFlatCommand(flatId);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
