using FluentValidation;

namespace FlatFlow.Application.Features.Tenant.Commands.UpdateTenantEmail;

public class UpdateTenantEmailCommandValidator : AbstractValidator<UpdateTenantEmailCommand>
{
    public UpdateTenantEmailCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
