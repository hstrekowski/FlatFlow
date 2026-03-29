using FluentValidation;

namespace FlatFlow.Application.Features.Payment.Commands.MarkShareAsPaid;

public class MarkShareAsPaidCommandValidator : AbstractValidator<MarkShareAsPaidCommand>
{
    public MarkShareAsPaidCommandValidator()
    {
        RuleFor(x => x.PaymentId).NotEmpty();
        RuleFor(x => x.ShareId).NotEmpty();
    }
}
