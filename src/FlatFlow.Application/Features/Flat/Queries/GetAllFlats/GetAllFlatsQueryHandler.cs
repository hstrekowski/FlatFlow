using AutoMapper;
using FlatFlow.Application.Common.Models;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Flat.Queries.DTOs;
using MediatR;

namespace FlatFlow.Application.Features.Flat.Queries.GetAllFlats;

public class GetAllFlatsQueryHandler : IRequestHandler<GetAllFlatsQuery, PaginatedResult<FlatDto>>
{
    private readonly IFlatRepository _flatRepository;
    private readonly IMapper _mapper;

    public GetAllFlatsQueryHandler(IFlatRepository flatRepository, IMapper mapper)
    {
        _flatRepository = flatRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<FlatDto>> Handle(GetAllFlatsQuery request, CancellationToken cancellationToken)
    {
        var result = await _flatRepository.GetAllPaginatedAsync(request.Page, request.PageSize, cancellationToken);
        var dtos = _mapper.Map<List<FlatDto>>(result.Items);

        return new PaginatedResult<FlatDto>(dtos, result.TotalCount, result.Page, result.PageSize);
    }
}
