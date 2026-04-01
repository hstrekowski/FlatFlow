using FlatFlow.Application.Common.Models.Identity;
using FlatFlow.Application.Contracts.Identity;
using MediatR;

namespace FlatFlow.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IAuthService _authService;

    public LoginCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await _authService.LoginAsync(new AuthRequest
        {
            Email = request.Email,
            Password = request.Password
        });
    }
}
