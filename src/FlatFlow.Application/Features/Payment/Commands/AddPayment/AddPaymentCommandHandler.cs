using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlatFlow.Application.Features.Payment.Commands.AddPayment;

public class AddPaymentCommandHandler : IRequestHandler<AddPaymentCommand, Guid>
{
    private readonly IFlatRepository _flatRepository;
    private readonly ILogger<AddPaymentCommandHandler> _logger;

    public AddPaymentCommandHandler(IFlatRepository flatRepository, ILogger<AddPaymentCommandHandler> logger)
    {
        _flatRepository = flatRepository;
        _logger = logger;
    }

    public async Task<Guid> Handle(AddPaymentCommand request, CancellationToken cancellationToken)
    {
        var flat = await _flatRepository.GetByIdWithPaymentsAsync(request.FlatId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Flat), request.FlatId);

        var payment = flat.AddPayment(request.Title, request.Amount, request.DueDate, request.CreatedById);

        await _flatRepository.UpdateAsync(flat, cancellationToken);

        _logger.LogInformation("Payment '{PaymentTitle}' added to Flat {FlatId}", request.Title, flat.Id);

        return payment.Id;
    }
}
