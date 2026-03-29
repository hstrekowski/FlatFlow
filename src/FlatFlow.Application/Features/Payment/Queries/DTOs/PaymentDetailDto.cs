namespace FlatFlow.Application.Features.Payment.Queries.DTOs;

public record PaymentDetailDto(
    Guid Id,
    string Title,
    decimal Amount,
    DateTime DueDate,
    Guid CreatedById,
    List<PaymentShareDto> Shares);
