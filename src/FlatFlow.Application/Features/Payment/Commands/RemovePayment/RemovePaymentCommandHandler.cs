using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Identity;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Payment.Commands.RemovePayment;

public class RemovePaymentCommandHandler : IRequestHandler<RemovePaymentCommand, Unit>
{
    private readonly IFlatRepository _flatRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<RemovePaymentCommandHandler> _logger;

    public RemovePaymentCommandHandler(
        IFlatRepository flatRepository,
        ITenantRepository tenantRepository,
        ICurrentUserService currentUserService,
        ILogger<RemovePaymentCommandHandler> logger)
    {
        _flatRepository = flatRepository;
        _tenantRepository = tenantRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Unit> Handle(RemovePaymentCommand request, CancellationToken cancellationToken)
    {
        var flat = await _flatRepository.GetByIdWithPaymentsAsync(request.FlatId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Flat), request.FlatId);

        var payment = flat.Payments.FirstOrDefault(p => p.Id == request.PaymentId)
            ?? throw new NotFoundException(nameof(Domain.Entities.Payment), request.PaymentId);

        var currentTenant = await _tenantRepository.GetByUserIdAndFlatIdAsync(
            _currentUserService.UserId, request.FlatId, cancellationToken)
            ?? throw new ForbiddenException("You are not a tenant in this flat.");

        if (!currentTenant.IsOwner && payment.CreatedById != currentTenant.Id)
            throw new ForbiddenException("You can only remove your own payments.");

        flat.RemovePayment(request.PaymentId);

        await _flatRepository.UpdateAsync(flat, cancellationToken);

        _logger.LogInformation("Payment {PaymentId} removed from Flat {FlatId}", request.PaymentId, flat.Id);

        return Unit.Value;
    }
}
