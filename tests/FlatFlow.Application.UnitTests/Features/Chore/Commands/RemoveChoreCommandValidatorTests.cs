using FlatFlow.Application.Features.Chore.Commands.RemoveChore;
using FluentAssertions;

namespace FlatFlow.Application.UnitTests.Features.Chore.Commands;

public class RemoveChoreCommandValidatorTests
{
    private readonly RemoveChoreCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new RemoveChoreCommand(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyFlatId_ShouldHaveError()
    {
        // Arrange
        var command = new RemoveChoreCommand(Guid.Empty, Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "FlatId");
    }

    [Fact]
    public async Task Validate_EmptyChoreId_ShouldHaveError()
    {
        // Arrange
        var command = new RemoveChoreCommand(Guid.NewGuid(), Guid.Empty);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "ChoreId");
    }
}
