using FlatFlow.Application.Features.Payment.Commands.AddPayment;
using FluentAssertions;

namespace FlatFlow.Application.UnitTests.Features.Payment.Commands;

public class AddPaymentCommandValidatorTests
{
    private readonly AddPaymentCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new AddPaymentCommand(Guid.NewGuid(), "Czynsz", 1500m, DateTime.UtcNow, Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyFlatId_ShouldHaveError()
    {
        // Arrange
        var command = new AddPaymentCommand(Guid.Empty, "Czynsz", 1500m, DateTime.UtcNow, Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "FlatId");
    }

    [Fact]
    public async Task Validate_EmptyTitle_ShouldHaveError()
    {
        // Arrange
        var command = new AddPaymentCommand(Guid.NewGuid(), "", 1500m, DateTime.UtcNow, Guid.NewGuid());

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
        var command = new AddPaymentCommand(Guid.NewGuid(), "Czynsz", 0m, DateTime.UtcNow, Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Amount");
    }

    [Fact]
    public async Task Validate_NegativeAmount_ShouldHaveError()
    {
        // Arrange
        var command = new AddPaymentCommand(Guid.NewGuid(), "Czynsz", -100m, DateTime.UtcNow, Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Amount");
    }

    [Fact]
    public async Task Validate_EmptyCreatedById_ShouldHaveError()
    {
        // Arrange
        var command = new AddPaymentCommand(Guid.NewGuid(), "Czynsz", 1500m, DateTime.UtcNow, Guid.Empty);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "CreatedById");
    }
}
