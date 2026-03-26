namespace FlatFlow.Application.Features.Payment.Queries.DTOs;

public record PaymentDto(Guid Id, string Title, decimal Amount, DateTime DueDate, Guid CreatedById);
