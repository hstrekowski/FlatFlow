using FlatFlow.Application.Contracts.Persistence;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace FlatFlow.Api.Authorization;

public class FlatAuthorizationHandler : IAuthorizationHandler
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FlatAuthorizationHandler(ITenantRepository tenantRepository, IHttpContextAccessor httpContextAccessor)
    {
        _tenantRepository = tenantRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return;

        var flatIdValue = httpContext.Request.RouteValues["flatId"]?.ToString()
            ?? httpContext.Request.RouteValues["id"]?.ToString();
        if (!Guid.TryParse(flatIdValue, out var flatId))
            return;

        var userId = context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? context.User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId))
            return;

        var tenants = await _tenantRepository.GetByFlatIdAsync(flatId);
        var tenant = tenants.FirstOrDefault(t => t.UserId == userId);

        foreach (var requirement in context.PendingRequirements.ToList())
        {
            if (requirement is FlatMemberRequirement && tenant != null)
            {
                context.Succeed(requirement);
            }
            else if (requirement is FlatOwnerRequirement && tenant is { IsOwner: true })
            {
                context.Succeed(requirement);
            }
        }
    }
}
