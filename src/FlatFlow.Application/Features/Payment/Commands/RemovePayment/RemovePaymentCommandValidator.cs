using FluentValidation;

namespace FlatFlow.Application.Features.Payment.Commands.RemovePayment;

public class RemovePaymentCommandValidator : AbstractValidator<RemovePaymentCommand>
{
    public RemovePaymentCommandValidator()
    {
        RuleFor(x => x.FlatId).NotEmpty();
        RuleFor(x => x.PaymentId).NotEmpty();
    }
}
