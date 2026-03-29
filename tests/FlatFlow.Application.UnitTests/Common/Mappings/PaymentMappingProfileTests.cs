using AutoMapper;
using FlatFlow.Application.Common.Mappings;
using FlatFlow.Application.Features.Payment.Queries.DTOs;
using FlatFlow.Domain.Enums;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace FlatFlow.Application.UnitTests.Common.Mappings;

public class PaymentMappingProfileTests
{
    private readonly IMapper _mapper;

    public PaymentMappingProfileTests()
    {
        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PaymentMappingProfile>();
        }, NullLoggerFactory.Instance));
    }

    [Fact]
    public void MappingProfile_ShouldHaveValidConfiguration()
    {
        // Arrange & Act & Assert
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    public void Map_PaymentToPaymentDto_ShouldMapCorrectly()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var createdById = Guid.NewGuid();
        var dueDate = DateTime.UtcNow.AddDays(30);
        var payment = flat.AddPayment("Czynsz", 1500m, dueDate, createdById);

        // Act
        var result = _mapper.Map<PaymentDto>(payment);

        // Assert
        result.Id.Should().Be(payment.Id);
        result.Title.Should().Be("Czynsz");
        result.Amount.Should().Be(1500m);
        result.DueDate.Should().Be(dueDate);
        result.CreatedById.Should().Be(createdById);
    }

    [Fact]
    public void Map_PaymentToPaymentDetailDto_ShouldMapWithShares()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var payment = flat.AddPayment("Czynsz", 1500m, DateTime.UtcNow, Guid.NewGuid());
        var tenantId = Guid.NewGuid();
        var share = payment.AddShare(tenantId, 500m);

        // Act
        var result = _mapper.Map<PaymentDetailDto>(payment);

        // Assert
        result.Id.Should().Be(payment.Id);
        result.Title.Should().Be("Czynsz");
        result.Shares.Should().ContainSingle();
        result.Shares[0].Id.Should().Be(share.Id);
        result.Shares[0].TenantId.Should().Be(tenantId);
        result.Shares[0].ShareAmount.Should().Be(500m);
        result.Shares[0].Status.Should().Be(PaymentShareStatus.New);
    }

    [Fact]
    public void Map_PaymentToPaymentDetailDto_ShouldMapEmptyShares()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var payment = flat.AddPayment("Czynsz", 1500m, DateTime.UtcNow, Guid.NewGuid());

        // Act
        var result = _mapper.Map<PaymentDetailDto>(payment);

        // Assert
        result.Shares.Should().BeEmpty();
    }
}
