using FlatFlow.Application.Features.Note.Commands.UpdateNote;
using FluentAssertions;

namespace FlatFlow.Application.UnitTests.Features.Note.Commands;

public class UpdateNoteCommandValidatorTests
{
    private readonly UpdateNoteCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new UpdateNoteCommand(Guid.NewGuid(), "Zakupy", "Kupić mleko");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyNoteId_ShouldHaveError()
    {
        // Arrange
        var command = new UpdateNoteCommand(Guid.Empty, "Zakupy", "Opis");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "NoteId");
    }

    [Fact]
    public async Task Validate_EmptyTitle_ShouldHaveError()
    {
        // Arrange
        var command = new UpdateNoteCommand(Guid.NewGuid(), "", "Opis");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Title");
    }
}
