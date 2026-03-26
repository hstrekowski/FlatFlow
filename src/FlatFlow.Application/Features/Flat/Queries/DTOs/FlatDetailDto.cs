using FlatFlow.Application.Features.Chore.Queries.DTOs;
using FlatFlow.Application.Features.Note.Queries.DTOs;
using FlatFlow.Application.Features.Payment.Queries.DTOs;
using FlatFlow.Application.Features.Tenant.Queries.DTOs;

namespace FlatFlow.Application.Features.Flat.Queries.DTOs;

public record FlatDetailDto(
    Guid Id,
    string Name,
    string Street,
    string City,
    string ZipCode,
    string Country,
    string AccessCode,
    List<TenantDto> Tenants,
    List<ChoreDto> Chores,
    List<PaymentDto> Payments,
    List<NoteDto> Notes);
