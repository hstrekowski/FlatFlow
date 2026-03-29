using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Payment.Commands.UpdatePayment;

public class UpdatePaymentCommandHandler : IRequestHandler<UpdatePaymentCommand, Unit>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger<UpdatePaymentCommandHandler> _logger;

    public UpdatePaymentCommandHandler(IPaymentRepository paymentRepository, ILogger<UpdatePaymentCommandHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdatePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Payment), request.PaymentId);

        payment.UpdateTitle(request.Title);
        payment.UpdateAmount(request.Amount);
        payment.UpdateDueDate(request.DueDate);

        await _paymentRepository.UpdateAsync(payment, cancellationToken);

        _logger.LogInformation("Payment {PaymentId} updated", payment.Id);

        return Unit.Value;
    }
}
