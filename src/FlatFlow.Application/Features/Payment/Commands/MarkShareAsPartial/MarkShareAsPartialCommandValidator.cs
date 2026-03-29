using FluentValidation;

namespace FlatFlow.Application.Features.Payment.Commands.MarkShareAsPartial;

public class MarkShareAsPartialCommandValidator : AbstractValidator<MarkShareAsPartialCommand>
{
    public MarkShareAsPartialCommandValidator()
    {
        RuleFor(x => x.PaymentId).NotEmpty();
        RuleFor(x => x.ShareId).NotEmpty();
    }
}
