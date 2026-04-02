using FlatFlow.Application.Features.Chore.Commands.AddChore;
using FlatFlow.Domain.Enums;
using FluentAssertions;

namespace FlatFlow.Application.UnitTests.Features.Chore.Commands;

public class AddChoreCommandValidatorTests
{
    private readonly AddChoreCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new AddChoreCommand(Guid.NewGuid(), "Sprzątanie", "Opis", ChoreFrequency.Weekly, Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyFlatId_ShouldHaveError()
    {
        // Arrange
        var command = new AddChoreCommand(Guid.Empty, "Sprzątanie", "Opis", ChoreFrequency.Weekly, Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "FlatId");
    }

    [Fact]
    public async Task Validate_EmptyTitle_ShouldHaveError()
    {
        // Arrange
        var command = new AddChoreCommand(Guid.NewGuid(), "", "Opis", ChoreFrequency.Weekly, Guid.NewGuid());

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
        var command = new AddChoreCommand(Guid.NewGuid(), "Sprzątanie", "Opis", (ChoreFrequency)999, Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Frequency");
    }
}
