using FluentValidation;

namespace FlatFlow.Application.Features.Chore.Commands.AddChoreAssignment;

public class AddChoreAssignmentCommandValidator : AbstractValidator<AddChoreAssignmentCommand>
{
    public AddChoreAssignmentCommandValidator()
    {
        RuleFor(x => x.ChoreId).NotEmpty();
        RuleFor(x => x.TenantId).NotEmpty();
    }
}
