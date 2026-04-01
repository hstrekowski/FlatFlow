using FlatFlow.Application.Common.Models.Identity;
using FlatFlow.Application.Contracts.Identity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IAuthService _authService;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(IAuthService authService, ILogger<RegisterCommandHandler> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var response = await _authService.RegisterAsync(new RegistrationRequest
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Password = request.Password
        });

        _logger.LogInformation("User '{Email}' registered with ID {UserId}", request.Email, response.UserId);

        return response;
    }
}
