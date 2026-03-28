using FlatFlow.Application.Features.Tenant.Commands.UpdateTenantEmail;
using FluentAssertions;

namespace FlatFlow.Application.UnitTests.Features.Tenant.Commands;

public class UpdateTenantEmailCommandValidatorTests
{
    private readonly UpdateTenantEmailCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new UpdateTenantEmailCommand(Guid.NewGuid(), "jan@test.com");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyTenantId_ShouldHaveError()
    {
        // Arrange
        var command = new UpdateTenantEmailCommand(Guid.Empty, "jan@test.com");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "TenantId");
    }

    [Fact]
    public async Task Validate_EmptyEmail_ShouldHaveError()
    {
        // Arrange
        var command = new UpdateTenantEmailCommand(Guid.NewGuid(), "");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public async Task Validate_InvalidEmail_ShouldHaveError()
    {
        // Arrange
        var command = new UpdateTenantEmailCommand(Guid.NewGuid(), "not-an-email");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Email");
    }
}
