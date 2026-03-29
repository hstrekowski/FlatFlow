using FluentValidation;

namespace FlatFlow.Application.Features.Payment.Commands.AddPaymentShare;

public class AddPaymentShareCommandValidator : AbstractValidator<AddPaymentShareCommand>
{
    public AddPaymentShareCommandValidator()
    {
        RuleFor(x => x.PaymentId).NotEmpty();
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.ShareAmount).GreaterThan(0);
    }
}
