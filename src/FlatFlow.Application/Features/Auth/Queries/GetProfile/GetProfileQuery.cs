using FlatFlow.Application.Features.Auth.Queries.DTOs;
using MediatR;

namespace FlatFlow.Application.Features.Auth.Queries.GetProfile;

public record GetProfileQuery(string UserId) : IRequest<ProfileDto>;
