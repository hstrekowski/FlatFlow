using AutoMapper;
using FlatFlow.Application.Common.Mappings;
using FlatFlow.Application.Features.Chore.Queries.DTOs;
using FlatFlow.Domain.Enums;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace FlatFlow.Application.UnitTests.Common.Mappings;

public class ChoreMappingProfileTests
{
    private readonly IMapper _mapper;

    public ChoreMappingProfileTests()
    {
        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ChoreMappingProfile>();
        }, NullLoggerFactory.Instance));
    }

    [Fact]
    public void MappingProfile_ShouldHaveValidConfiguration()
    {
        // Arrange & Act & Assert
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    public void Map_ChoreToChoreDto_ShouldMapCorrectly()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var chore = flat.AddChore("Sprzątanie", "Posprzątać kuchnię", ChoreFrequency.Weekly);

        // Act
        var result = _mapper.Map<ChoreDto>(chore);

        // Assert
        result.Id.Should().Be(chore.Id);
        result.Title.Should().Be("Sprzątanie");
        result.Description.Should().Be("Posprzątać kuchnię");
        result.Frequency.Should().Be(ChoreFrequency.Weekly);
    }

    [Fact]
    public void Map_ChoreToChoreDetailDto_ShouldMapWithAssignments()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var chore = flat.AddChore("Sprzątanie", "Opis", ChoreFrequency.Weekly);
        var tenantId = Guid.NewGuid();
        var dueDate = DateTime.UtcNow.AddDays(7);
        var assignment = chore.AddAssignment(tenantId, dueDate);

        // Act
        var result = _mapper.Map<ChoreDetailDto>(chore);

        // Assert
        result.Id.Should().Be(chore.Id);
        result.Title.Should().Be("Sprzątanie");
        result.Assignments.Should().ContainSingle();
        result.Assignments[0].Id.Should().Be(assignment.Id);
        result.Assignments[0].TenantId.Should().Be(tenantId);
        result.Assignments[0].DueDate.Should().Be(dueDate);
        result.Assignments[0].IsCompleted.Should().BeFalse();
        result.Assignments[0].CompletedAt.Should().BeNull();
    }

    [Fact]
    public void Map_ChoreToChoreDetailDto_ShouldMapEmptyAssignments()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var chore = flat.AddChore("Sprzątanie", "Opis", ChoreFrequency.Once);

        // Act
        var result = _mapper.Map<ChoreDetailDto>(chore);

        // Assert
        result.Assignments.Should().BeEmpty();
    }
}
