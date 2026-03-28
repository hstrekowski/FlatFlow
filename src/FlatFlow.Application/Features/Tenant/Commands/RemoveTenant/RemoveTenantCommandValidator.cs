using FluentValidation;

namespace FlatFlow.Application.Features.Tenant.Commands.RemoveTenant;

public class RemoveTenantCommandValidator : AbstractValidator<RemoveTenantCommand>
{
    public RemoveTenantCommandValidator()
    {
        RuleFor(x => x.FlatId).NotEmpty();
        RuleFor(x => x.TenantId).NotEmpty();
    }
}
