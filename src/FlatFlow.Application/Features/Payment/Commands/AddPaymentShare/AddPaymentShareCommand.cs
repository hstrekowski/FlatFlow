using MediatR;

namespace FlatFlow.Application.Features.Payment.Commands.AddPaymentShare;

public record AddPaymentShareCommand(
    Guid PaymentId,
    Guid TenantId,
    decimal ShareAmount) : IRequest<Guid>;
