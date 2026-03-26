using MediatR;

namespace FlatFlow.Application.Features.Flat.Commands.CreateFlat;

public record CreateFlatCommand(
    string Name,
    string Street,
    string City,
    string ZipCode,
    string Country) : IRequest<Guid>;
