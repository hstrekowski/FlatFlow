namespace FlatFlow.Application.Features.Note.Queries.DTOs;

public record NoteDto(Guid Id, string Title, string Content, Guid AuthorId, DateTime CreatedAt);
