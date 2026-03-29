using FluentValidation;

namespace FlatFlow.Application.Features.Chore.Commands.RemoveChoreAssignment;

public class RemoveChoreAssignmentCommandValidator : AbstractValidator<RemoveChoreAssignmentCommand>
{
    public RemoveChoreAssignmentCommandValidator()
    {
        RuleFor(x => x.ChoreId).NotEmpty();
        RuleFor(x => x.AssignmentId).NotEmpty();
    }
}
