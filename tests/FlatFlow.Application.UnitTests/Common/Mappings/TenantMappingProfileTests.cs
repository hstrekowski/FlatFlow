using AutoMapper;
using FlatFlow.Application.Common.Mappings;
using FlatFlow.Application.Features.Tenant.Queries.DTOs;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace FlatFlow.Application.UnitTests.Common.Mappings;

public class TenantMappingProfileTests
{
    private readonly IMapper _mapper;

    public TenantMappingProfileTests()
    {
        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<TenantMappingProfile>();
        }, NullLoggerFactory.Instance));
    }

    [Fact]
    public void Configuration_ShouldBeValid()
    {
        // Arrange & Act & Assert
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    public void Map_Tenant_ToTenantDto_ShouldMapAllProperties()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var tenant = flat.AddTenant("Jan", "Kowalski", "jan@test.com", "user-1", true);

        // Act
        var dto = _mapper.Map<TenantDto>(tenant);

        // Assert
        dto.Id.Should().Be(tenant.Id);
        dto.FirstName.Should().Be("Jan");
        dto.LastName.Should().Be("Kowalski");
        dto.Email.Should().Be("jan@test.com");
        dto.IsOwner.Should().BeTrue();
    }
}
