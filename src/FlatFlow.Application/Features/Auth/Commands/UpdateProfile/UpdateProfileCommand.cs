using MediatR;

namespace FlatFlow.Application.Features.Auth.Commands.UpdateProfile;

public record UpdateProfileCommand(
    string FirstName,
    string LastName,
    string Email) : IRequest<Unit>
{
    public string UserId { get; init; } = string.Empty;
}
