using FlatFlow.Application.Features.Tenant.Commands.RemoveTenant;
using FluentAssertions;

namespace FlatFlow.Application.UnitTests.Features.Tenant.Commands;

public class RemoveTenantCommandValidatorTests
{
    private readonly RemoveTenantCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new RemoveTenantCommand(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyFlatId_ShouldHaveError()
    {
        // Arrange
        var command = new RemoveTenantCommand(Guid.Empty, Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "FlatId");
    }

    [Fact]
    public async Task Validate_EmptyTenantId_ShouldHaveError()
    {
        // Arrange
        var command = new RemoveTenantCommand(Guid.NewGuid(), Guid.Empty);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "TenantId");
    }
}
