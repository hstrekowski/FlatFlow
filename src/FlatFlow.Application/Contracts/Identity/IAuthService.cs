using FlatFlow.Application.Common.Models.Identity;

namespace FlatFlow.Application.Contracts.Identity
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegistrationRequest request);
        Task<AuthResponse> LoginAsync(AuthRequest request);
        Task<UserProfile> GetUserAsync(string userId);
        Task UpdateUserAsync(string userId, string firstName, string lastName, string email);
    }
}
