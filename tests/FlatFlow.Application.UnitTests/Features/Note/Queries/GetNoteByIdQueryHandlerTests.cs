using AutoMapper;
using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Common.Mappings;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Note.Queries.GetNoteById;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace FlatFlow.Application.UnitTests.Features.Note.Queries;

public class GetNoteByIdQueryHandlerTests
{
    private readonly Mock<INoteRepository> _noteRepositoryMock;
    private readonly GetNoteByIdQueryHandler _handler;

    public GetNoteByIdQueryHandlerTests()
    {
        _noteRepositoryMock = new Mock<INoteRepository>();
        var mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<NoteMappingProfile>();
        }, NullLoggerFactory.Instance));

        _handler = new GetNoteByIdQueryHandler(_noteRepositoryMock.Object, mapper);
    }

    [Fact]
    public async Task Handle_ExistingNote_ShouldReturnNoteDto()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var authorId = Guid.NewGuid();
        var note = flat.AddNote("Zakupy", "Kupić mleko", authorId);

        _noteRepositoryMock
            .Setup(r => r.GetByIdAsync(note.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(note);

        var query = new GetNoteByIdQuery(note.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Id.Should().Be(note.Id);
        result.Title.Should().Be("Zakupy");
        result.Content.Should().Be("Kupić mleko");
        result.AuthorId.Should().Be(authorId);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Handle_NonExistingNote_ShouldThrowNotFoundException()
    {
        // Arrange
        var noteId = Guid.NewGuid();
        _noteRepositoryMock
            .Setup(r => r.GetByIdAsync(noteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Note?)null);

        var query = new GetNoteByIdQuery(noteId);

        // Act
        var act = () => _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
