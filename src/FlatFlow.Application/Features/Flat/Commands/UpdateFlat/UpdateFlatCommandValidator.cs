using FluentValidation;

namespace FlatFlow.Application.Features.Flat.Commands.UpdateFlat;

public class UpdateFlatCommandValidator : AbstractValidator<UpdateFlatCommand>
{
    public UpdateFlatCommandValidator()
    {
        RuleFor(x => x.FlatId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Street).NotEmpty();
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.ZipCode).NotEmpty();
        RuleFor(x => x.Country).NotEmpty();
    }
}
