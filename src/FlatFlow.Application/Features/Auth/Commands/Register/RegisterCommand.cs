using FlatFlow.Application.Common.Models.Identity;
using MediatR;

namespace FlatFlow.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password) : IRequest<AuthResponse>;
