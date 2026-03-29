using MediatR;

namespace FlatFlow.Application.Features.Payment.Commands.UpdatePayment;

public record UpdatePaymentCommand(
    Guid PaymentId,
    string Title,
    decimal Amount,
    DateTime DueDate) : IRequest<Unit>;
