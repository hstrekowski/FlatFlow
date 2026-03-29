using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Note.Commands.AddNote;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlatFlow.Application.UnitTests.Features.Note.Commands;

public class AddNoteCommandHandlerTests
{
    private readonly Mock<IFlatRepository> _flatRepositoryMock;
    private readonly AddNoteCommandHandler _handler;

    public AddNoteCommandHandlerTests()
    {
        _flatRepositoryMock = new Mock<IFlatRepository>();
        _handler = new AddNoteCommandHandler(
            _flatRepositoryMock.Object,
            Mock.Of<ILogger<AddNoteCommandHandler>>());
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldReturnNoteIdAndAddToFlat()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var authorId = Guid.NewGuid();
        _flatRepositoryMock
            .Setup(r => r.GetByIdWithNotesAsync(flat.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(flat);

        var command = new AddNoteCommand(flat.Id, "Zakupy", "Kupić mleko", authorId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        var addedNote = flat.Notes.Should().ContainSingle().Subject;
        result.Should().Be(addedNote.Id);
        addedNote.Title.Should().Be("Zakupy");
        addedNote.Content.Should().Be("Kupić mleko");
        addedNote.AuthorId.Should().Be(authorId);
        _flatRepositoryMock.Verify(r => r.UpdateAsync(flat, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingFlat_ShouldThrowNotFoundException()
    {
        // Arrange
        var flatId = Guid.NewGuid();
        _flatRepositoryMock
            .Setup(r => r.GetByIdWithNotesAsync(flatId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Flat?)null);

        var command = new AddNoteCommand(flatId, "Zakupy", "Opis", Guid.NewGuid());

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
