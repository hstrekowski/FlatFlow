using MediatR;

namespace FlatFlow.Application.Features.Payment.Commands.MarkShareAsPaid;

public record MarkShareAsPaidCommand(Guid PaymentId, Guid ShareId) : IRequest<Unit>;
