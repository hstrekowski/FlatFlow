using FluentValidation;

namespace FlatFlow.Application.Features.Note.Commands.RemoveNote;

public class RemoveNoteCommandValidator : AbstractValidator<RemoveNoteCommand>
{
    public RemoveNoteCommandValidator()
    {
        RuleFor(x => x.FlatId).NotEmpty();
        RuleFor(x => x.NoteId).NotEmpty();
    }
}
