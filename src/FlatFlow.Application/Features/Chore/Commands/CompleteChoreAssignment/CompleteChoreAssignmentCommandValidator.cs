using FluentValidation;

namespace FlatFlow.Application.Features.Chore.Commands.CompleteChoreAssignment;

public class CompleteChoreAssignmentCommandValidator : AbstractValidator<CompleteChoreAssignmentCommand>
{
    public CompleteChoreAssignmentCommandValidator()
    {
        RuleFor(x => x.ChoreId).NotEmpty();
        RuleFor(x => x.AssignmentId).NotEmpty();
    }
}
