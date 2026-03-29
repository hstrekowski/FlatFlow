using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Payment.Commands.RemovePaymentShare;

public class RemovePaymentShareCommandHandler : IRequestHandler<RemovePaymentShareCommand, Unit>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger<RemovePaymentShareCommandHandler> _logger;

    public RemovePaymentShareCommandHandler(IPaymentRepository paymentRepository, ILogger<RemovePaymentShareCommandHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(RemovePaymentShareCommand request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdWithSharesAsync(request.PaymentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Payment), request.PaymentId);

        payment.RemoveShare(request.ShareId);

        await _paymentRepository.UpdateAsync(payment, cancellationToken);

        _logger.LogInformation("Share {ShareId} removed from Payment {PaymentId}", request.ShareId, payment.Id);

        return Unit.Value;
    }
}
