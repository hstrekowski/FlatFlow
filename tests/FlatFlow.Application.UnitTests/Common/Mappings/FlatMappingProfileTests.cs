using AutoMapper;
using FlatFlow.Application.Common.Mappings;
using FlatFlow.Application.Features.Flat.Queries.DTOs;
using FlatFlow.Domain.Entities;
using FlatFlow.Domain.Enums;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace FlatFlow.Application.UnitTests.Common.Mappings;

public class FlatMappingProfileTests
{
    private readonly IMapper _mapper;
    private static readonly ILoggerFactory LogFactory = NullLoggerFactory.Instance;

    public FlatMappingProfileTests()
    {
        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<FlatMappingProfile>();
            cfg.AddProfile<TenantMappingProfile>();
            cfg.AddProfile<ChoreMappingProfile>();
            cfg.AddProfile<PaymentMappingProfile>();
            cfg.AddProfile<NoteMappingProfile>();
        }, LogFactory));
    }

    [Fact]
    public void MappingProfile_ShouldHaveValidConfiguration()
    {
        // Arrange
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<FlatMappingProfile>();
            cfg.AddProfile<TenantMappingProfile>();
            cfg.AddProfile<ChoreMappingProfile>();
            cfg.AddProfile<PaymentMappingProfile>();
            cfg.AddProfile<NoteMappingProfile>();
        }, LogFactory);

        // Act & Assert
        config.AssertConfigurationIsValid();
    }

    [Fact]
    public void Map_FlatToFlatDto_ShouldMapCorrectly()
    {
        // Arrange
        var address = new Address("Długa 5", "Kraków", "30-001", "Poland");
        var flat = new Flat("Mieszkanie", address);

        // Act
        var dto = _mapper.Map<FlatDto>(flat);

        // Assert
        dto.Id.Should().Be(flat.Id);
        dto.Name.Should().Be("Mieszkanie");
        dto.City.Should().Be("Kraków");
        dto.AccessCode.Should().Be(flat.AccessCode);
    }

    [Fact]
    public void Map_FlatToFlatDetailDto_ShouldMapAddressFields()
    {
        // Arrange
        var address = new Address("Długa 5", "Kraków", "30-001", "Poland");
        var flat = new Flat("Mieszkanie", address);

        // Act
        var dto = _mapper.Map<FlatDetailDto>(flat);

        // Assert
        dto.Id.Should().Be(flat.Id);
        dto.Name.Should().Be("Mieszkanie");
        dto.Street.Should().Be("Długa 5");
        dto.City.Should().Be("Kraków");
        dto.ZipCode.Should().Be("30-001");
        dto.Country.Should().Be("Poland");
        dto.AccessCode.Should().Be(flat.AccessCode);
    }

    [Fact]
    public void Map_FlatToFlatDetailDto_ShouldMapEmptyCollections()
    {
        // Arrange
        var address = new Address("Długa 5", "Kraków", "30-001", "Poland");
        var flat = new Flat("Mieszkanie", address);

        // Act
        var dto = _mapper.Map<FlatDetailDto>(flat);

        // Assert
        dto.Tenants.Should().BeEmpty();
        dto.Chores.Should().BeEmpty();
        dto.Payments.Should().BeEmpty();
        dto.Notes.Should().BeEmpty();
    }

    [Fact]
    public void Map_FlatWithChildren_ToFlatDetailDto_ShouldMapChildCollections()
    {
        // Arrange
        var address = new Address("Długa 5", "Kraków", "30-001", "Poland");
        var flat = new Flat("Mieszkanie", address);
        var tenant = flat.AddTenant("Jan", "Kowalski", "jan@mail.com", "user-1");
        flat.AddChore("Sprzątanie", "Opis", ChoreFrequency.Weekly);
        flat.AddPayment("Czynsz", 1000m, DateTime.UtcNow, tenant.Id);
        flat.AddNote("Notatka", "Treść", tenant.Id);

        // Act
        var dto = _mapper.Map<FlatDetailDto>(flat);

        // Assert
        dto.Tenants.Should().HaveCount(1);
        dto.Tenants[0].FirstName.Should().Be("Jan");
        dto.Chores.Should().HaveCount(1);
        dto.Chores[0].Title.Should().Be("Sprzątanie");
        dto.Payments.Should().HaveCount(1);
        dto.Payments[0].Title.Should().Be("Czynsz");
        dto.Notes.Should().HaveCount(1);
        dto.Notes[0].Title.Should().Be("Notatka");
    }
}
