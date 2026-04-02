using FluentValidation;

namespace FlatFlow.Application.Features.Tenant.Commands.JoinFlat;

public class JoinFlatCommandValidator : AbstractValidator<JoinFlatCommand>
{
    public JoinFlatCommandValidator()
    {
        RuleFor(x => x.AccessCode).NotEmpty();
    }
}
