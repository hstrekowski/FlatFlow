using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Flat.Commands.CreateFlat;
using FlatFlow.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlatFlow.Application.UnitTests.Features.Flat.Commands;

public class CreateFlatCommandHandlerTests
{
    private readonly Mock<IFlatRepository> _flatRepositoryMock;
    private readonly CreateFlatCommandHandler _handler;

    public CreateFlatCommandHandlerTests()
    {
        _flatRepositoryMock = new Mock<IFlatRepository>();
        _flatRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Flat>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Flat f, CancellationToken _) => f);

        _handler = new CreateFlatCommandHandler(
            _flatRepositoryMock.Object,
            Mock.Of<ILogger<CreateFlatCommandHandler>>());
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldReturnGuid()
    {
        // Arrange
        var command = new CreateFlatCommand("Mieszkanie", "Długa 5", "Kraków", "30-001", "Poland");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCallAddAsync()
    {
        // Arrange
        var command = new CreateFlatCommand("Mieszkanie", "Długa 5", "Kraków", "30-001", "Poland");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _flatRepositoryMock.Verify(
            r => r.AddAsync(It.Is<Domain.Entities.Flat>(f =>
                f.Name == "Mieszkanie" &&
                f.Address.City == "Kraków"),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
