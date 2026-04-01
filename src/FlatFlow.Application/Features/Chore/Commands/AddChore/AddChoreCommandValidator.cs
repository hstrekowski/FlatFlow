using FluentValidation;

namespace FlatFlow.Application.Features.Chore.Commands.AddChore;

public class AddChoreCommandValidator : AbstractValidator<AddChoreCommand>
{
    public AddChoreCommandValidator()
    {
        RuleFor(x => x.FlatId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Frequency).IsInEnum();
        RuleFor(x => x.CreatedById).NotEmpty();
    }
}
