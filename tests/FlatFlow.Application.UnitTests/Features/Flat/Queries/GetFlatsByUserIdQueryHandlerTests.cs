using AutoMapper;
using FlatFlow.Application.Common.Mappings;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Flat.Queries.GetFlatsByUserId;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace FlatFlow.Application.UnitTests.Features.Flat.Queries;

public class GetFlatsByUserIdQueryHandlerTests
{
    private readonly Mock<IFlatRepository> _flatRepositoryMock;
    private readonly GetFlatsByUserIdQueryHandler _handler;

    public GetFlatsByUserIdQueryHandlerTests()
    {
        _flatRepositoryMock = new Mock<IFlatRepository>();
        var mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<FlatMappingProfile>();
        }, NullLoggerFactory.Instance));

        _handler = new GetFlatsByUserIdQueryHandler(_flatRepositoryMock.Object, mapper);
    }

    [Fact]
    public async Task Handle_UserWithFlats_ShouldReturnFlatDtos()
    {
        // Arrange
        var flat1 = new Domain.Entities.Flat("Mieszkanie 1", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var flat2 = new Domain.Entities.Flat("Mieszkanie 2", new Address("Krótka 3", "Warszawa", "00-001", "Poland"));

        _flatRepositoryMock
            .Setup(r => r.GetByTenantUserIdAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync([flat1, flat2]);

        var query = new GetFlatsByUserIdQuery("user-1");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Mieszkanie 1");
        result[1].Name.Should().Be("Mieszkanie 2");
    }

    [Fact]
    public async Task Handle_UserWithNoFlats_ShouldReturnEmptyList()
    {
        // Arrange
        _flatRepositoryMock
            .Setup(r => r.GetByTenantUserIdAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var query = new GetFlatsByUserIdQuery("user-1");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}
