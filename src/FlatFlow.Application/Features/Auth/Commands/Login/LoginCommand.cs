using FlatFlow.Application.Common.Models.Identity;
using MediatR;

namespace FlatFlow.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password) : IRequest<AuthResponse>;
