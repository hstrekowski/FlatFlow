using AutoMapper;
using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Flat.Queries.DTOs;
using MediatR;

namespace FlatFlow.Application.Features.Flat.Queries.GetFlatById;

public class GetFlatByIdQueryHandler : IRequestHandler<GetFlatByIdQuery, FlatDetailDto>
{
    private readonly IFlatRepository _flatRepository;
    private readonly IMapper _mapper;

    public GetFlatByIdQueryHandler(IFlatRepository flatRepository, IMapper mapper)
    {
        _flatRepository = flatRepository;
        _mapper = mapper;
    }

    public async Task<FlatDetailDto> Handle(GetFlatByIdQuery request, CancellationToken cancellationToken)
    {
        var flat = await _flatRepository.GetByIdWithAllAsync(request.FlatId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Flat), request.FlatId);

        return _mapper.Map<FlatDetailDto>(flat);
    }
}
