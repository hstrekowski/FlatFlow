using FlatFlow.Application.Features.Chore.Commands.RemoveChoreAssignment;
using FluentAssertions;

namespace FlatFlow.Application.UnitTests.Features.Chore.Commands;

public class RemoveChoreAssignmentCommandValidatorTests
{
    private readonly RemoveChoreAssignmentCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new RemoveChoreAssignmentCommand(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyChoreId_ShouldHaveError()
    {
        // Arrange
        var command = new RemoveChoreAssignmentCommand(Guid.Empty, Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "ChoreId");
    }

    [Fact]
    public async Task Validate_EmptyAssignmentId_ShouldHaveError()
    {
        // Arrange
        var command = new RemoveChoreAssignmentCommand(Guid.NewGuid(), Guid.Empty);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "AssignmentId");
    }
}
