using AutoMapper;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Chore.Queries.DTOs;
using MediatR;

namespace FlatFlow.Application.Features.Chore.Queries.GetChoresByFlatId;

public class GetChoresByFlatIdQueryHandler : IRequestHandler<GetChoresByFlatIdQuery, List<ChoreDto>>
{
    private readonly IChoreRepository _choreRepository;
    private readonly IMapper _mapper;

    public GetChoresByFlatIdQueryHandler(IChoreRepository choreRepository, IMapper mapper)
    {
        _choreRepository = choreRepository;
        _mapper = mapper;
    }

    public async Task<List<ChoreDto>> Handle(GetChoresByFlatIdQuery request, CancellationToken cancellationToken)
    {
        var chores = await _choreRepository.GetByFlatIdAsync(request.FlatId, cancellationToken);

        return _mapper.Map<List<ChoreDto>>(chores);
    }
}
