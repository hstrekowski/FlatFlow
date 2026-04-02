using FlatFlow.Application.Features.Flat.Queries.DTOs;
using MediatR;

namespace FlatFlow.Application.Features.Flat.Queries.GetMyFlats;

public record GetMyFlatsQuery() : IRequest<List<FlatDto>>;
