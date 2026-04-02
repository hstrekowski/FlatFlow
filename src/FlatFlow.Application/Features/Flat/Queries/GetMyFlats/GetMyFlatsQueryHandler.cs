using AutoMapper;
using FlatFlow.Application.Contracts.Identity;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Flat.Queries.DTOs;
using MediatR;

namespace FlatFlow.Application.Features.Flat.Queries.GetMyFlats;

public class GetMyFlatsQueryHandler : IRequestHandler<GetMyFlatsQuery, List<FlatDto>>
{
    private readonly IFlatRepository _flatRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetMyFlatsQueryHandler(
        IFlatRepository flatRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _flatRepository = flatRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<List<FlatDto>> Handle(GetMyFlatsQuery request, CancellationToken cancellationToken)
    {
        var flats = await _flatRepository.GetByTenantUserIdAsync(_currentUserService.UserId, cancellationToken);

        return _mapper.Map<List<FlatDto>>(flats);
    }
}
