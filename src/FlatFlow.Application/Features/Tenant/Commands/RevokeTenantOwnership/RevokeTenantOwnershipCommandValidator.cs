using FluentValidation;

namespace FlatFlow.Application.Features.Tenant.Commands.RevokeTenantOwnership;

public class RevokeTenantOwnershipCommandValidator : AbstractValidator<RevokeTenantOwnershipCommand>
{
    public RevokeTenantOwnershipCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
    }
}
