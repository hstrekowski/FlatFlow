using FlatFlow.Application.Features.Flat.Queries.DTOs;
using MediatR;

namespace FlatFlow.Application.Features.Flat.Queries.GetFlatById;

public record GetFlatByIdQuery(Guid FlatId) : IRequest<FlatDetailDto>;
