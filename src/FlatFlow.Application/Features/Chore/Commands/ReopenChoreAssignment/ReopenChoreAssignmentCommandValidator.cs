using FluentValidation;

namespace FlatFlow.Application.Features.Chore.Commands.ReopenChoreAssignment;

public class ReopenChoreAssignmentCommandValidator : AbstractValidator<ReopenChoreAssignmentCommand>
{
    public ReopenChoreAssignmentCommandValidator()
    {
        RuleFor(x => x.ChoreId).NotEmpty();
        RuleFor(x => x.AssignmentId).NotEmpty();
    }
}
