using FluentValidation;

namespace FlatFlow.Application.Features.Chore.Commands.UpdateChore;

public class UpdateChoreCommandValidator : AbstractValidator<UpdateChoreCommand>
{
    public UpdateChoreCommandValidator()
    {
        RuleFor(x => x.ChoreId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Frequency).IsInEnum();
    }
}
