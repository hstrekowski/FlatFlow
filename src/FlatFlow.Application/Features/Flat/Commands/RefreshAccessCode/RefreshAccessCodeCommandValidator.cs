using FluentValidation;

namespace FlatFlow.Application.Features.Flat.Commands.RefreshAccessCode;

public class RefreshAccessCodeCommandValidator : AbstractValidator<RefreshAccessCodeCommand>
{
    public RefreshAccessCodeCommandValidator()
    {
        RuleFor(x => x.FlatId).NotEmpty();
    }
}
