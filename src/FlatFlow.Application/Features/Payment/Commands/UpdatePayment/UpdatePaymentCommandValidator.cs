using FluentValidation;

namespace FlatFlow.Application.Features.Payment.Commands.UpdatePayment;

public class UpdatePaymentCommandValidator : AbstractValidator<UpdatePaymentCommand>
{
    public UpdatePaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}
