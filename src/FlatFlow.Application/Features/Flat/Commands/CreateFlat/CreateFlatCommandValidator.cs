using FluentValidation;

namespace FlatFlow.Application.Features.Flat.Commands.CreateFlat;

public class CreateFlatCommandValidator : AbstractValidator<CreateFlatCommand>
{
    public CreateFlatCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Street).NotEmpty();
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.ZipCode).NotEmpty();
        RuleFor(x => x.Country).NotEmpty();
    }
}
