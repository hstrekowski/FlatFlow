using FluentValidation;

namespace FlatFlow.Application.Features.Chore.Commands.RemoveChore;

public class RemoveChoreCommandValidator : AbstractValidator<RemoveChoreCommand>
{
    public RemoveChoreCommandValidator()
    {
        RuleFor(x => x.FlatId).NotEmpty();
        RuleFor(x => x.ChoreId).NotEmpty();
    }
}
