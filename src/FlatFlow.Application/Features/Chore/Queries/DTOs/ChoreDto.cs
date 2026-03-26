using FlatFlow.Domain.Enums;

namespace FlatFlow.Application.Features.Chore.Queries.DTOs;

public record ChoreDto(Guid Id, string Title, string Description, ChoreFrequency Frequency);
