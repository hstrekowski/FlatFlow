using FlatFlow.Application.Common.Behaviors;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlatFlow.Application.UnitTests.Common.Behaviors;

public class LoggingBehaviorTests
{
    private readonly Mock<ILogger<LoggingBehavior<TestRequest, string>>> _loggerMock;
    private readonly Mock<RequestHandlerDelegate<string>> _nextMock;
    private readonly LoggingBehavior<TestRequest, string> _behavior;

    public LoggingBehaviorTests()
    {
        _loggerMock = new Mock<ILogger<LoggingBehavior<TestRequest, string>>>();
        _nextMock = new Mock<RequestHandlerDelegate<string>>();
        _nextMock.Setup(n => n()).ReturnsAsync("handler-result");
        _behavior = new LoggingBehavior<TestRequest, string>(_loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCallNextAndReturnResponse()
    {
        // Arrange
        var request = new TestRequest("test");

        // Act
        var result = await _behavior.Handle(request, _nextMock.Object, CancellationToken.None);

        // Assert
        result.Should().Be("handler-result");
        _nextMock.Verify(n => n(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogBeforeAndAfterHandler()
    {
        // Arrange
        var request = new TestRequest("test");

        // Act
        await _behavior.Handle(request, _nextMock.Object, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Handling")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Handled")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNextThrows_ShouldNotLogHandled()
    {
        // Arrange
        var request = new TestRequest("test");
        _nextMock.Setup(n => n()).ThrowsAsync(new InvalidOperationException("boom"));

        // Act
        var act = () => _behavior.Handle(request, _nextMock.Object, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Handling")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Handled")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    public record TestRequest(string Name) : IRequest<string>;
}
