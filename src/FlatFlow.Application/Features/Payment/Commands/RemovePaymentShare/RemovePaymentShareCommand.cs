using MediatR;

namespace FlatFlow.Application.Features.Payment.Commands.RemovePaymentShare;

public record RemovePaymentShareCommand(Guid PaymentId, Guid ShareId) : IRequest<Unit>;
