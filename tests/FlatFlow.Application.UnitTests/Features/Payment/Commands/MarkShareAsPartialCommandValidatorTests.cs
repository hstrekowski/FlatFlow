using FlatFlow.Application.Features.Payment.Commands.MarkShareAsPartial;
using FluentAssertions;

namespace FlatFlow.Application.UnitTests.Features.Payment.Commands;

public class MarkShareAsPartialCommandValidatorTests
{
    private readonly MarkShareAsPartialCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new MarkShareAsPartialCommand(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyPaymentId_ShouldHaveError()
    {
        // Arrange
        var command = new MarkShareAsPartialCommand(Guid.Empty, Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "PaymentId");
    }

    [Fact]
    public async Task Validate_EmptyShareId_ShouldHaveError()
    {
        // Arrange
        var command = new MarkShareAsPartialCommand(Guid.NewGuid(), Guid.Empty);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "ShareId");
    }
}
