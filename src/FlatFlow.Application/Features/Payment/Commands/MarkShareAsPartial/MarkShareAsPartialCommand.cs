using MediatR;

namespace FlatFlow.Application.Features.Payment.Commands.MarkShareAsPartial;

public record MarkShareAsPartialCommand(Guid PaymentId, Guid ShareId) : IRequest<Unit>;
