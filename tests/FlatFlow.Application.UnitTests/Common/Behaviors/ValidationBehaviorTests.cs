using FlatFlow.Application.Common.Behaviors;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;

namespace FlatFlow.Application.UnitTests.Common.Behaviors;

public class ValidationBehaviorTests
{
    private readonly Mock<RequestHandlerDelegate<string>> _nextMock;

    public ValidationBehaviorTests()
    {
        _nextMock = new Mock<RequestHandlerDelegate<string>>();
        _nextMock.Setup(n => n()).ReturnsAsync("handler-result");
    }

    [Fact]
    public async Task Handle_NoValidators_ShouldCallNext()
    {
        // Arrange
        var validators = Enumerable.Empty<IValidator<TestRequest>>();
        var behavior = new ValidationBehavior<TestRequest, string>(validators);
        var request = new TestRequest("valid");

        // Act
        var result = await behavior.Handle(request, _nextMock.Object, CancellationToken.None);

        // Assert
        result.Should().Be("handler-result");
        _nextMock.Verify(n => n(), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidatorsWithNoErrors_ShouldCallNext()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var behavior = new ValidationBehavior<TestRequest, string>(new[] { validatorMock.Object });
        var request = new TestRequest("valid");

        // Act
        var result = await behavior.Handle(request, _nextMock.Object, CancellationToken.None);

        // Assert
        result.Should().Be("handler-result");
        _nextMock.Verify(n => n(), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidatorsWithErrors_ShouldThrowValidationException()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new("Name", "Name is required"),
            new("City", "City is required")
        };

        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var behavior = new ValidationBehavior<TestRequest, string>(new[] { validatorMock.Object });
        var request = new TestRequest("");

        // Act
        var act = () => behavior.Handle(request, _nextMock.Object, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<ValidationException>();
        exception.Which.Errors.Should().HaveCount(2);
        _nextMock.Verify(n => n(), Times.Never);
    }

    [Fact]
    public async Task Handle_MultipleValidatorsWithMixedResults_ShouldThrowWithAllFailures()
    {
        // Arrange
        var validator1Mock = new Mock<IValidator<TestRequest>>();
        validator1Mock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("Name", "Name is required") }));

        var validator2Mock = new Mock<IValidator<TestRequest>>();
        validator2Mock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("City", "City is required") }));

        var behavior = new ValidationBehavior<TestRequest, string>(new[] { validator1Mock.Object, validator2Mock.Object });
        var request = new TestRequest("");

        // Act
        var act = () => behavior.Handle(request, _nextMock.Object, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<ValidationException>();
        exception.Which.Errors.Should().HaveCount(2);
        _nextMock.Verify(n => n(), Times.Never);
    }

    public record TestRequest(string Name) : IRequest<string>;
}
