using FlatFlow.Application.Features.Payment.Commands.AddPaymentShare;
using FluentAssertions;

namespace FlatFlow.Application.UnitTests.Features.Payment.Commands;

public class AddPaymentShareCommandValidatorTests
{
    private readonly AddPaymentShareCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new AddPaymentShareCommand(Guid.NewGuid(), Guid.NewGuid(), 500m);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyPaymentId_ShouldHaveError()
    {
        // Arrange
        var command = new AddPaymentShareCommand(Guid.Empty, Guid.NewGuid(), 500m);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "PaymentId");
    }

    [Fact]
    public async Task Validate_EmptyTenantId_ShouldHaveError()
    {
        // Arrange
        var command = new AddPaymentShareCommand(Guid.NewGuid(), Guid.Empty, 500m);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "TenantId");
    }

    [Fact]
    public async Task Validate_ZeroShareAmount_ShouldHaveError()
    {
        // Arrange
        var command = new AddPaymentShareCommand(Guid.NewGuid(), Guid.NewGuid(), 0m);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "ShareAmount");
    }
}
