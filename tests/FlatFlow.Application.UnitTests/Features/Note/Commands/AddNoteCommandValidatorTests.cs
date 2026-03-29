using FlatFlow.Application.Features.Note.Commands.AddNote;
using FluentAssertions;

namespace FlatFlow.Application.UnitTests.Features.Note.Commands;

public class AddNoteCommandValidatorTests
{
    private readonly AddNoteCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new AddNoteCommand(Guid.NewGuid(), "Zakupy", "Kupić mleko", Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyFlatId_ShouldHaveError()
    {
        // Arrange
        var command = new AddNoteCommand(Guid.Empty, "Zakupy", "Opis", Guid.NewGuid());

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
        var command = new AddNoteCommand(Guid.NewGuid(), "", "Opis", Guid.NewGuid());

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Title");
    }

    [Fact]
    public async Task Validate_EmptyAuthorId_ShouldHaveError()
    {
        // Arrange
        var command = new AddNoteCommand(Guid.NewGuid(), "Zakupy", "Opis", Guid.Empty);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "AuthorId");
    }
}
