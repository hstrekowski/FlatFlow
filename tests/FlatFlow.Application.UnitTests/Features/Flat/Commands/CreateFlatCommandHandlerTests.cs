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
    private Domain.Entities.Flat? _capturedFlat;

    public CreateFlatCommandHandlerTests()
    {
        _flatRepositoryMock = new Mock<IFlatRepository>();
        _flatRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Flat>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Flat f, CancellationToken _) =>
            {
                _capturedFlat = f;
                return f;
            });

        _handler = new CreateFlatCommandHandler(
            _flatRepositoryMock.Object,
            Mock.Of<ILogger<CreateFlatCommandHandler>>());
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldReturnCreatedFlatId()
    {
        // Arrange
        var command = new CreateFlatCommand("Mieszkanie", "Długa 5", "Kraków", "30-001", "Poland");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _capturedFlat.Should().NotBeNull();
        result.Should().Be(_capturedFlat!.Id);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateFlatWithCorrectProperties()
    {
        // Arrange
        var command = new CreateFlatCommand("Mieszkanie", "Długa 5", "Kraków", "30-001", "Poland");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _flatRepositoryMock.Verify(
            r => r.AddAsync(It.Is<Domain.Entities.Flat>(f =>
                f.Name == "Mieszkanie" &&
                f.Address.Street == "Długa 5" &&
                f.Address.City == "Kraków" &&
                f.Address.ZipCode == "30-001" &&
                f.Address.Country == "Poland" &&
                f.AccessCode != string.Empty),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
