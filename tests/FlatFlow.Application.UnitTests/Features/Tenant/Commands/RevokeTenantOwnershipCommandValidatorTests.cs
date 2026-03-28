using FlatFlow.Application.Features.Tenant.Commands.RevokeTenantOwnership;
using FluentAssertions;

namespace FlatFlow.Application.UnitTests.Features.Tenant.Commands;

public class RevokeTenantOwnershipCommandValidatorTests
{
    private readonly RevokeTenantOwnershipCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new RevokeTenantOwnershipCommand(Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyTenantId_ShouldHaveError()
    {
        // Arrange
        var command = new RevokeTenantOwnershipCommand(Guid.Empty);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "TenantId");
    }
}
