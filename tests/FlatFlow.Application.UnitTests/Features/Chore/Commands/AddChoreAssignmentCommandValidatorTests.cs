using FlatFlow.Application.Features.Chore.Commands.AddChoreAssignment;
using FluentAssertions;

namespace FlatFlow.Application.UnitTests.Features.Chore.Commands;

public class AddChoreAssignmentCommandValidatorTests
{
    private readonly AddChoreAssignmentCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new AddChoreAssignmentCommand(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(7));

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyChoreId_ShouldHaveError()
    {
        // Arrange
        var command = new AddChoreAssignmentCommand(Guid.Empty, Guid.NewGuid(), DateTime.UtcNow.AddDays(7));

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "ChoreId");
    }

    [Fact]
    public async Task Validate_EmptyTenantId_ShouldHaveError()
    {
        // Arrange
        var command = new AddChoreAssignmentCommand(Guid.NewGuid(), Guid.Empty, DateTime.UtcNow.AddDays(7));

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "TenantId");
    }
}
