using FluentValidation;

namespace FlatFlow.Application.Features.Payment.Commands.RemovePaymentShare;

public class RemovePaymentShareCommandValidator : AbstractValidator<RemovePaymentShareCommand>
{
    public RemovePaymentShareCommandValidator()
    {
        RuleFor(x => x.PaymentId).NotEmpty();
        RuleFor(x => x.ShareId).NotEmpty();
    }
}
