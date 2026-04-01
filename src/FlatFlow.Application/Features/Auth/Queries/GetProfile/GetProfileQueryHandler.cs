using FlatFlow.Application.Contracts.Identity;
using FlatFlow.Application.Features.Auth.Queries.DTOs;
using MediatR;

namespace FlatFlow.Application.Features.Auth.Queries.GetProfile;

public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, ProfileDto>
{
    private readonly IAuthService _authService;

    public GetProfileQueryHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<ProfileDto> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _authService.GetUserAsync(request.UserId);

        return new ProfileDto(user.UserId, user.Email, user.FirstName, user.LastName);
    }
}
