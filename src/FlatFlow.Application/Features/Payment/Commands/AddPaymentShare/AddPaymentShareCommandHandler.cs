using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Payment.Commands.AddPaymentShare;

public class AddPaymentShareCommandHandler : IRequestHandler<AddPaymentShareCommand, Guid>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger<AddPaymentShareCommandHandler> _logger;

    public AddPaymentShareCommandHandler(IPaymentRepository paymentRepository, ILogger<AddPaymentShareCommandHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _logger = logger;
    }

    public async Task<Guid> Handle(AddPaymentShareCommand request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdWithSharesAsync(request.PaymentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Payment), request.PaymentId);

        var share = payment.AddShare(request.TenantId, request.ShareAmount);

        await _paymentRepository.UpdateAsync(payment, cancellationToken);

        _logger.LogInformation("Share added to Payment {PaymentId} for Tenant {TenantId}", payment.Id, request.TenantId);

        return share.Id;
    }
}
