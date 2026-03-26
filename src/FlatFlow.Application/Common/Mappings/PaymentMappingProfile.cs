using AutoMapper;
using FlatFlow.Application.Features.Payment.Queries.DTOs;

namespace FlatFlow.Application.Common.Mappings;

public class PaymentMappingProfile : Profile
{
    public PaymentMappingProfile()
    {
        CreateMap<Domain.Entities.Payment, PaymentDto>();
    }
}
