using FlatFlow.Application.Features.Tenant.Commands.JoinFlat;
using FluentAssertions;

namespace FlatFlow.Application.UnitTests.Features.Tenant.Commands;

public class JoinFlatCommandValidatorTests
{
    private readonly JoinFlatCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new JoinFlatCommand("ABC123");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyAccessCode_ShouldHaveError()
    {
        // Arrange
        var command = new JoinFlatCommand("");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AccessCode");
    }
}
