using MediatR;

namespace FlatFlow.Application.Features.Flat.Commands.UpdateFlat;

public record UpdateFlatCommand(
    Guid FlatId,
    string Name,
    string Street,
    string City,
    string ZipCode,
    string Country) : IRequest<Unit>;
