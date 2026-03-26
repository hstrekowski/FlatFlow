using FlatFlow.Application.Common.Behaviors;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlatFlow.Application.UnitTests.Common.Behaviors;

public class UnhandledExceptionBehaviorTests
{
    private readonly Mock<ILogger<UnhandledExceptionBehavior<TestRequest, string>>> _loggerMock;
    private readonly Mock<RequestHandlerDelegate<string>> _nextMock;
    private readonly UnhandledExceptionBehavior<TestRequest, string> _behavior;

    public UnhandledExceptionBehaviorTests()
    {
        _loggerMock = new Mock<ILogger<UnhandledExceptionBehavior<TestRequest, string>>>();
        _nextMock = new Mock<RequestHandlerDelegate<string>>();
        _behavior = new UnhandledExceptionBehavior<TestRequest, string>(_loggerMock.Object);
    }

    [Fact]
    public async Task Handle_NoException_ShouldReturnResponse()
    {
        // Arrange
        _nextMock.Setup(n => n()).ReturnsAsync("handler-result");
        var request = new TestRequest("test");

        // Act
        var result = await _behavior.Handle(request, _nextMock.Object, CancellationToken.None);

        // Assert
        result.Should().Be("handler-result");
        _nextMock.Verify(n => n(), Times.Once);
    }

    [Fact]
    public async Task Handle_NoException_ShouldNotLogError()
    {
        // Arrange
        _nextMock.Setup(n => n()).ReturnsAsync("handler-result");
        var request = new TestRequest("test");

        // Act
        await _behavior.Handle(request, _nextMock.Object, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ExceptionThrown_ShouldLogErrorAndRethrow()
    {
        // Arrange
        var exception = new InvalidOperationException("something broke");
        _nextMock.Setup(n => n()).ThrowsAsync(exception);
        var request = new TestRequest("test");

        // Act
        var act = () => _behavior.Handle(request, _nextMock.Object, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("something broke");

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Unhandled exception")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ExceptionThrown_ShouldRethrowSameException()
    {
        // Arrange
        var exception = new InvalidOperationException("original");
        _nextMock.Setup(n => n()).ThrowsAsync(exception);
        var request = new TestRequest("test");

        // Act
        var act = () => _behavior.Handle(request, _nextMock.Object, CancellationToken.None);

        // Assert
        (await act.Should().ThrowAsync<InvalidOperationException>())
            .Which.Should().BeSameAs(exception);
    }

    public record TestRequest(string Name) : IRequest<string>;
}
