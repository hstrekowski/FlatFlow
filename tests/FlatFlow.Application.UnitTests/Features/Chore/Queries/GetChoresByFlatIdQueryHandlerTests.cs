using AutoMapper;
using FlatFlow.Application.Common.Mappings;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Chore.Queries.GetChoresByFlatId;
using FlatFlow.Domain.Enums;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace FlatFlow.Application.UnitTests.Features.Chore.Queries;

public class GetChoresByFlatIdQueryHandlerTests
{
    private readonly Mock<IChoreRepository> _choreRepositoryMock;
    private readonly GetChoresByFlatIdQueryHandler _handler;

    public GetChoresByFlatIdQueryHandlerTests()
    {
        _choreRepositoryMock = new Mock<IChoreRepository>();
        var mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ChoreMappingProfile>();
        }, NullLoggerFactory.Instance));

        _handler = new GetChoresByFlatIdQueryHandler(_choreRepositoryMock.Object, mapper);
    }

    [Fact]
    public async Task Handle_ExistingFlat_ShouldReturnChoreDtos()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var chore1 = flat.AddChore("Sprzątanie", "Opis 1", ChoreFrequency.Weekly);
        var chore2 = flat.AddChore("Gotowanie", "Opis 2", ChoreFrequency.Daily);

        _choreRepositoryMock
            .Setup(r => r.GetByFlatIdAsync(flat.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync([chore1, chore2]);

        var query = new GetChoresByFlatIdQuery(flat.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(c => c.Title == "Sprzątanie" && c.Frequency == ChoreFrequency.Weekly);
        result.Should().Contain(c => c.Title == "Gotowanie" && c.Frequency == ChoreFrequency.Daily);
    }

    [Fact]
    public async Task Handle_NoChores_ShouldReturnEmptyList()
    {
        // Arrange
        var flatId = Guid.NewGuid();
        _choreRepositoryMock
            .Setup(r => r.GetByFlatIdAsync(flatId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var query = new GetChoresByFlatIdQuery(flatId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}
