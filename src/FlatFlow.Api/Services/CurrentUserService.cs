using System.Security.Claims;
using FlatFlow.Application.Contracts.Identity;

namespace FlatFlow.Api.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public string UserId { get; }

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            UserId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }
    }
}
