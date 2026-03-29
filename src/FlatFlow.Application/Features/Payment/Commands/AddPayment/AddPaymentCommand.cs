using MediatR;

namespace FlatFlow.Application.Features.Payment.Commands.AddPayment;

public record AddPaymentCommand(
    Guid FlatId,
    string Title,
    decimal Amount,
    DateTime DueDate,
    Guid CreatedById) : IRequest<Guid>;
