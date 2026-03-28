using FluentValidation;

namespace FlatFlow.Application.Features.Tenant.Commands.PromoteTenant;

public class PromoteTenantCommandValidator : AbstractValidator<PromoteTenantCommand>
{
    public PromoteTenantCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
    }
}
