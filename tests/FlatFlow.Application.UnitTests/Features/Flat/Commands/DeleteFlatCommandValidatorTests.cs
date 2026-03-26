using FlatFlow.Application.Features.Flat.Commands.DeleteFlat;
using FluentAssertions;

namespace FlatFlow.Application.UnitTests.Features.Flat.Commands;

public class DeleteFlatCommandValidatorTests
{
    private readonly DeleteFlatCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new DeleteFlatCommand(Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyFlatId_ShouldHaveError()
    {
        // Arrange
        var command = new DeleteFlatCommand(Guid.Empty);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "FlatId");
    }
}
