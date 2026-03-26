using AutoMapper;
using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Flat.Queries.DTOs;
using MediatR;

namespace FlatFlow.Application.Features.Flat.Queries.GetFlatByAccessCode;

public class GetFlatByAccessCodeQueryHandler : IRequestHandler<GetFlatByAccessCodeQuery, FlatDto>
{
    private readonly IFlatRepository _flatRepository;
    private readonly IMapper _mapper;

    public GetFlatByAccessCodeQueryHandler(IFlatRepository flatRepository, IMapper mapper)
    {
        _flatRepository = flatRepository;
        _mapper = mapper;
    }

    public async Task<FlatDto> Handle(GetFlatByAccessCodeQuery request, CancellationToken cancellationToken)
    {
        var flat = await _flatRepository.GetByAccessCodeAsync(request.AccessCode, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Flat), request.AccessCode);

        return _mapper.Map<FlatDto>(flat);
    }
}
