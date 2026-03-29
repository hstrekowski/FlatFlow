using MediatR;

namespace FlatFlow.Application.Features.Payment.Commands.RemovePayment;

public record RemovePaymentCommand(Guid FlatId, Guid PaymentId) : IRequest<Unit>;
