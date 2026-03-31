namespace FlatFlow.Application.Common.Models.Identity
{
    public class AuthResponse
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
