using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Identity;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Payment.Commands.MarkShareAsPartial;

public class MarkShareAsPartialCommandHandler : IRequestHandler<MarkShareAsPartialCommand, Unit>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<MarkShareAsPartialCommandHandler> _logger;

    public MarkShareAsPartialCommandHandler(
        IPaymentRepository paymentRepository,
        ITenantRepository tenantRepository,
        ICurrentUserService currentUserService,
        ILogger<MarkShareAsPartialCommandHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _tenantRepository = tenantRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Unit> Handle(MarkShareAsPartialCommand request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdWithSharesAsync(request.PaymentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Payment), request.PaymentId);

        var share = payment.PaymentShares.FirstOrDefault(s => s.Id == request.ShareId)
            ?? throw new NotFoundException(nameof(Domain.Entities.PaymentShare), request.ShareId);

        var currentTenant = await _tenantRepository.GetByUserIdAndFlatIdAsync(
            _currentUserService.UserId, payment.FlatId, cancellationToken)
            ?? throw new ForbiddenException("You are not a tenant in this flat.");

        if (!currentTenant.IsOwner && share.TenantId != currentTenant.Id)
            throw new ForbiddenException("You can only mark your own shares as partial.");

        share.MarkAsPartial();

        await _paymentRepository.UpdateAsync(payment, cancellationToken);

        _logger.LogInformation("Share {ShareId} marked as partial in Payment {PaymentId}", request.ShareId, payment.Id);

        return Unit.Value;
    }
}
