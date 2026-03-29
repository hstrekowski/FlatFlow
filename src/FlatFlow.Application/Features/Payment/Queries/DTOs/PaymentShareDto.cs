using FlatFlow.Domain.Enums;

namespace FlatFlow.Application.Features.Payment.Queries.DTOs;

public record PaymentShareDto(
    Guid Id,
    Guid TenantId,
    decimal ShareAmount,
    PaymentShareStatus Status);
