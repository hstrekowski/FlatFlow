using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Identity;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Payment.Commands.UpdatePayment;

public class UpdatePaymentCommandHandler : IRequestHandler<UpdatePaymentCommand, Unit>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdatePaymentCommandHandler> _logger;

    public UpdatePaymentCommandHandler(
        IPaymentRepository paymentRepository,
        ITenantRepository tenantRepository,
        ICurrentUserService currentUserService,
        ILogger<UpdatePaymentCommandHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _tenantRepository = tenantRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdatePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Payment), request.PaymentId);

        var currentTenant = await _tenantRepository.GetByUserIdAndFlatIdAsync(
            _currentUserService.UserId, payment.FlatId, cancellationToken)
            ?? throw new ForbiddenException("You are not a tenant in this flat.");

        if (!currentTenant.IsOwner && payment.CreatedById != currentTenant.Id)
            throw new ForbiddenException("You can only modify your own payments.");

        payment.UpdateTitle(request.Title);
        payment.UpdateAmount(request.Amount);
        payment.UpdateDueDate(request.DueDate);

        await _paymentRepository.UpdateAsync(payment, cancellationToken);

        _logger.LogInformation("Payment {PaymentId} updated", payment.Id);

        return Unit.Value;
    }
}
