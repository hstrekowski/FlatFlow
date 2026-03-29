using FlatFlow.Application.Features.Payment.Commands.UpdatePayment;
using FluentAssertions;

namespace FlatFlow.Application.UnitTests.Features.Payment.Commands;

public class UpdatePaymentCommandValidatorTests
{
    private readonly UpdatePaymentCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new UpdatePaymentCommand(Guid.NewGuid(), "Czynsz", 1500m, DateTime.UtcNow);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyPaymentId_ShouldHaveError()
    {
        // Arrange
        var command = new UpdatePaymentCommand(Guid.Empty, "Czynsz", 1500m, DateTime.UtcNow);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "PaymentId");
    }

    [Fact]
    public async Task Validate_EmptyTitle_ShouldHaveError()
    {
        // Arrange
        var command = new UpdatePaymentCommand(Guid.NewGuid(), "", 1500m, DateTime.UtcNow);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Title");
    }

    [Fact]
    public async Task Validate_ZeroAmount_ShouldHaveError()
    {
        // Arrange
        var command = new UpdatePaymentCommand(Guid.NewGuid(), "Czynsz", 0m, DateTime.UtcNow);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Amount");
    }
}
