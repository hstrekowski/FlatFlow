using FlatFlow.Application.Features.Flat.Queries.DTOs;
using MediatR;

namespace FlatFlow.Application.Features.Flat.Queries.GetFlatByAccessCode;

public record GetFlatByAccessCodeQuery(string AccessCode) : IRequest<FlatDto>;
