using MediatR;

namespace FlatFlow.Application.Features.Auth.Commands.UpdateProfile;

public record UpdateProfileCommand(
    string UserId,
    string FirstName,
    string LastName,
    string Email) : IRequest<Unit>;
