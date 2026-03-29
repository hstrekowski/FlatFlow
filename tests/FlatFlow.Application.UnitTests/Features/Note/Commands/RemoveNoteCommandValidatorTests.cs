using FlatFlow.Application.Features.Note.Commands.RemoveNote;
using FluentAssertions;

namespace FlatFlow.Application.UnitTests.Features.Note.Commands;

public class RemoveNoteCommandValidatorTests
{
    private readonly RemoveNoteCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new RemoveNoteCommand(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyFlatId_ShouldHaveError()
    {
        // Arrange
        var command = new RemoveNoteCommand(Guid.Empty, Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "FlatId");
    }

    [Fact]
    public async Task Validate_EmptyNoteId_ShouldHaveError()
    {
        // Arrange
        var command = new RemoveNoteCommand(Guid.NewGuid(), Guid.Empty);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "NoteId");
    }
}
