using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Identity;
using FlatFlow.Application.Common.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FlatFlow.Infrastructure.Identity
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<AuthResponse> RegisterAsync(RegistrationRequest request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                throw new AuthenticationException($"User with email '{request.Email}' already exists.");

            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                throw new AuthenticationException($"Registration failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            return new AuthResponse
            {
                UserId = user.Id,
                Email = user.Email!,
                Token = GenerateJwtToken(user)
            };
        }

        public async Task<AuthResponse> LoginAsync(AuthRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new AuthenticationException("Invalid email or password.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
                throw new AuthenticationException("Invalid email or password.");

            return new AuthResponse
            {
                UserId = user.Id,
                Email = user.Email!,
                Token = GenerateJwtToken(user)
            };
        }

        public async Task<UserProfile> GetUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new AuthenticationException($"User with ID '{userId}' not found.");

            return new UserProfile
            {
                UserId = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        public async Task UpdateUserAsync(string userId, string firstName, string lastName, string email)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new AuthenticationException($"User with ID '{userId}' not found.");

            user.FirstName = firstName;
            user.LastName = lastName;
            user.Email = email;
            user.UserName = email;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new AuthenticationException($"Profile update failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim("firstName", user.FirstName),
                new Claim("lastName", user.LastName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
