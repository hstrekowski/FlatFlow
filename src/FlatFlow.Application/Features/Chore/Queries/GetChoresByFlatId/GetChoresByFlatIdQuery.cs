using FlatFlow.Application.Features.Chore.Queries.DTOs;
using MediatR;

namespace FlatFlow.Application.Features.Chore.Queries.GetChoresByFlatId;

public record GetChoresByFlatIdQuery(Guid FlatId) : IRequest<List<ChoreDto>>;
