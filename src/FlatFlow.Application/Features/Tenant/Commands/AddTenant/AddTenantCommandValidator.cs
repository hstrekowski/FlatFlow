using FluentValidation;

namespace FlatFlow.Application.Features.Tenant.Commands.AddTenant;

public class AddTenantCommandValidator : AbstractValidator<AddTenantCommand>
{
    public AddTenantCommandValidator()
    {
        RuleFor(x => x.FlatId).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
