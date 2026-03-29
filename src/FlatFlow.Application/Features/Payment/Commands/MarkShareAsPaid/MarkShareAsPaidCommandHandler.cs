using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Payment.Commands.MarkShareAsPaid;

public class MarkShareAsPaidCommandHandler : IRequestHandler<MarkShareAsPaidCommand, Unit>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger<MarkShareAsPaidCommandHandler> _logger;

    public MarkShareAsPaidCommandHandler(IPaymentRepository paymentRepository, ILogger<MarkShareAsPaidCommandHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(MarkShareAsPaidCommand request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdWithSharesAsync(request.PaymentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Payment), request.PaymentId);

        var share = payment.PaymentShares.FirstOrDefault(s => s.Id == request.ShareId)
            ?? throw new NotFoundException(nameof(Domain.Entities.PaymentShare), request.ShareId);

        share.MarkAsPaid();

        await _paymentRepository.UpdateAsync(payment, cancellationToken);

        _logger.LogInformation("Share {ShareId} marked as paid in Payment {PaymentId}", request.ShareId, payment.Id);

        return Unit.Value;
    }
}
