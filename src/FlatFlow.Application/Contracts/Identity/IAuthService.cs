using FlatFlow.Application.Common.Models.Identity;

namespace FlatFlow.Application.Contracts.Identity
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegistrationRequest request);
        Task<AuthResponse> LoginAsync(AuthRequest request);
    }
}
