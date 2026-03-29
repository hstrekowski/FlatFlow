using FlatFlow.Application.Features.Payment.Queries.DTOs;
using MediatR;

namespace FlatFlow.Application.Features.Payment.Queries.GetPaymentById;

public record GetPaymentByIdQuery(Guid PaymentId) : IRequest<PaymentDetailDto>;
