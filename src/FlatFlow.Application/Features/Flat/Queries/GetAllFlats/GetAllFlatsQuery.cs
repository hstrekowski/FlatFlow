using FlatFlow.Application.Common.Models;
using FlatFlow.Application.Features.Flat.Queries.DTOs;
using MediatR;

namespace FlatFlow.Application.Features.Flat.Queries.GetAllFlats;

public record GetAllFlatsQuery(int Page, int PageSize) : IRequest<PaginatedResult<FlatDto>>;
