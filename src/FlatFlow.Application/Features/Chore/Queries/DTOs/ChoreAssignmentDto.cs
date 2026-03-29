namespace FlatFlow.Application.Features.Chore.Queries.DTOs;

public record ChoreAssignmentDto(
    Guid Id,
    Guid TenantId,
    DateTime DueDate,
    DateTime? CompletedAt,
    bool IsCompleted);
