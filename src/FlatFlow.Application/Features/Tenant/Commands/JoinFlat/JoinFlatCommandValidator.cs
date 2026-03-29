using FluentValidation;

namespace FlatFlow.Application.Features.Tenant.Commands.JoinFlat;

public class JoinFlatCommandValidator : AbstractValidator<JoinFlatCommand>
{
    public JoinFlatCommandValidator()
    {
        RuleFor(x => x.AccessCode).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
