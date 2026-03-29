using FlatFlow.Application.Features.Chore.Commands.UpdateChore;
using FlatFlow.Domain.Enums;
using FluentAssertions;

namespace FlatFlow.Application.UnitTests.Features.Chore.Commands;

public class UpdateChoreCommandValidatorTests
{
    private readonly UpdateChoreCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new UpdateChoreCommand(Guid.NewGuid(), "Sprzątanie", "Opis", ChoreFrequency.Weekly);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyChoreId_ShouldHaveError()
    {
        // Arrange
        var command = new UpdateChoreCommand(Guid.Empty, "Sprzątanie", "Opis", ChoreFrequency.Weekly);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "ChoreId");
    }

    [Fact]
    public async Task Validate_EmptyTitle_ShouldHaveError()
    {
        // Arrange
        var command = new UpdateChoreCommand(Guid.NewGuid(), "", "Opis", ChoreFrequency.Weekly);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Title");
    }

    [Fact]
    public async Task Validate_InvalidFrequency_ShouldHaveError()
    {
        // Arrange
        var command = new UpdateChoreCommand(Guid.NewGuid(), "Sprzątanie", "Opis", (ChoreFrequency)999);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Frequency");
    }
}
