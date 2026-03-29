using AutoMapper;
using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Chore.Queries.DTOs;
using MediatR;

namespace FlatFlow.Application.Features.Chore.Queries.GetChoreById;

public class GetChoreByIdQueryHandler : IRequestHandler<GetChoreByIdQuery, ChoreDetailDto>
{
    private readonly IChoreRepository _choreRepository;
    private readonly IMapper _mapper;

    public GetChoreByIdQueryHandler(IChoreRepository choreRepository, IMapper mapper)
    {
        _choreRepository = choreRepository;
        _mapper = mapper;
    }

    public async Task<ChoreDetailDto> Handle(GetChoreByIdQuery request, CancellationToken cancellationToken)
    {
        var chore = await _choreRepository.GetByIdWithAssignmentsAsync(request.ChoreId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Chore), request.ChoreId);

        return _mapper.Map<ChoreDetailDto>(chore);
    }
}
