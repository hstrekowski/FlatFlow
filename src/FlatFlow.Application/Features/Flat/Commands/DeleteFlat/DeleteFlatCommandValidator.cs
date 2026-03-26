using FluentValidation;

namespace FlatFlow.Application.Features.Flat.Commands.DeleteFlat;

public class DeleteFlatCommandValidator : AbstractValidator<DeleteFlatCommand>
{
    public DeleteFlatCommandValidator()
    {
        RuleFor(x => x.FlatId).NotEmpty();
    }
}
