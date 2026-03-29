using FlatFlow.Application.Features.Chore.Queries.DTOs;
using MediatR;

namespace FlatFlow.Application.Features.Chore.Queries.GetChoreById;

public record GetChoreByIdQuery(Guid ChoreId) : IRequest<ChoreDetailDto>;
