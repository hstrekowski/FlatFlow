using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Payment.Commands.RemovePayment;

public class RemovePaymentCommandHandler : IRequestHandler<RemovePaymentCommand, Unit>
{
    private readonly IFlatRepository _flatRepository;
    private readonly ILogger<RemovePaymentCommandHandler> _logger;

    public RemovePaymentCommandHandler(IFlatRepository flatRepository, ILogger<RemovePaymentCommandHandler> logger)
    {
        _flatRepository = flatRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(RemovePaymentCommand request, CancellationToken cancellationToken)
    {
        var flat = await _flatRepository.GetByIdWithPaymentsAsync(request.FlatId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Flat), request.FlatId);

        flat.RemovePayment(request.PaymentId);

        await _flatRepository.UpdateAsync(flat, cancellationToken);

        _logger.LogInformation("Payment {PaymentId} removed from Flat {FlatId}", request.PaymentId, flat.Id);

        return Unit.Value;
    }
}
