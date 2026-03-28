using FlatFlow.Application.Features.Flat.Queries.DTOs;
using MediatR;

namespace FlatFlow.Application.Features.Flat.Queries.GetFlatsByUserId;

public record GetFlatsByUserIdQuery(string UserId) : IRequest<List<FlatDto>>;
