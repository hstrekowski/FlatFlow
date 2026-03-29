using FlatFlow.Application.Common.Models;
using FlatFlow.Application.Features.Payment.Queries.DTOs;
using MediatR;

namespace FlatFlow.Application.Features.Payment.Queries.GetPaymentsByFlatId;

public record GetPaymentsByFlatIdQuery(Guid FlatId, int Page, int PageSize) : IRequest<PaginatedResult<PaymentDto>>;
