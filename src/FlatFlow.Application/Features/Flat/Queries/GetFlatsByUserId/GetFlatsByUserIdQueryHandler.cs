using AutoMapper;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Flat.Queries.DTOs;
using MediatR;

namespace FlatFlow.Application.Features.Flat.Queries.GetFlatsByUserId;

public class GetFlatsByUserIdQueryHandler : IRequestHandler<GetFlatsByUserIdQuery, List<FlatDto>>
{
    private readonly IFlatRepository _flatRepository;
    private readonly IMapper _mapper;

    public GetFlatsByUserIdQueryHandler(IFlatRepository flatRepository, IMapper mapper)
    {
        _flatRepository = flatRepository;
        _mapper = mapper;
    }

    public async Task<List<FlatDto>> Handle(GetFlatsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var flats = await _flatRepository.GetByTenantUserIdAsync(request.UserId, cancellationToken);

        return _mapper.Map<List<FlatDto>>(flats);
    }
}
