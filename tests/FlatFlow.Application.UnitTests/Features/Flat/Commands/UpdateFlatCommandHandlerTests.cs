using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Flat.Commands.UpdateFlat;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlatFlow.Application.UnitTests.Features.Flat.Commands;

public class UpdateFlatCommandHandlerTests
{
    private readonly Mock<IFlatRepository> _flatRepositoryMock;
    private readonly UpdateFlatCommandHandler _handler;

    public UpdateFlatCommandHandlerTests()
    {
        _flatRepositoryMock = new Mock<IFlatRepository>();
        _handler = new UpdateFlatCommandHandler(
            _flatRepositoryMock.Object,
            Mock.Of<ILogger<UpdateFlatCommandHandler>>());
    }

    [Fact]
    public async Task Handle_ExistingFlat_ShouldUpdateAndReturnUnit()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Old Name", new Address("Old St", "Old City", "00-000", "Old Country"));
        _flatRepositoryMock
            .Setup(r => r.GetByIdAsync(flat.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(flat);

        var command = new UpdateFlatCommand(flat.Id, "New Name", "New St", "New City", "11-111", "New Country");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        flat.Name.Should().Be("New Name");
        flat.Address.Street.Should().Be("New St");
        flat.Address.City.Should().Be("New City");
        flat.Address.ZipCode.Should().Be("11-111");
        flat.Address.Country.Should().Be("New Country");
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

        var command = new UpdateFlatCommand(flatId, "Name", "Street", "City", "00-000", "Country");

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
