using AutoMapper;
using FlatFlow.Application.Common.Models;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Payment.Queries.DTOs;
using MediatR;

namespace FlatFlow.Application.Features.Payment.Queries.GetPaymentsByFlatId;

public class GetPaymentsByFlatIdQueryHandler : IRequestHandler<GetPaymentsByFlatIdQuery, PaginatedResult<PaymentDto>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;

    public GetPaymentsByFlatIdQueryHandler(IPaymentRepository paymentRepository, IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<PaymentDto>> Handle(GetPaymentsByFlatIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _paymentRepository.GetByFlatIdPaginatedAsync(request.FlatId, request.Page, request.PageSize, cancellationToken);
        var dtos = _mapper.Map<List<PaymentDto>>(result.Items);

        return new PaginatedResult<PaymentDto>(dtos, result.TotalCount, result.Page, result.PageSize);
    }
}
