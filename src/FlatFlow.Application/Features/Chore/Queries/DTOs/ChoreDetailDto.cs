using FlatFlow.Domain.Enums;

namespace FlatFlow.Application.Features.Chore.Queries.DTOs;

public record ChoreDetailDto(
    Guid Id,
    string Title,
    string Description,
    ChoreFrequency Frequency,
    Guid CreatedById,
    List<ChoreAssignmentDto> Assignments);
