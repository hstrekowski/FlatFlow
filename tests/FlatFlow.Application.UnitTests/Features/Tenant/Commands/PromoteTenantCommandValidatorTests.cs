using FlatFlow.Application.Features.Tenant.Commands.PromoteTenant;
using FluentAssertions;

namespace FlatFlow.Application.UnitTests.Features.Tenant.Commands;

public class PromoteTenantCommandValidatorTests
{
    private readonly PromoteTenantCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new PromoteTenantCommand(Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyTenantId_ShouldHaveError()
    {
        // Arrange
        var command = new PromoteTenantCommand(Guid.Empty);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "TenantId");
    }
}
