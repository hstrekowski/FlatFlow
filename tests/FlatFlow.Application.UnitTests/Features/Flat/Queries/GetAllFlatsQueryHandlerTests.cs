using AutoMapper;
using FlatFlow.Application.Common.Mappings;
using FlatFlow.Application.Common.Models;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Flat.Queries.GetAllFlats;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace FlatFlow.Application.UnitTests.Features.Flat.Queries;

public class GetAllFlatsQueryHandlerTests
{
    private readonly Mock<IFlatRepository> _flatRepositoryMock;
    private readonly GetAllFlatsQueryHandler _handler;

    public GetAllFlatsQueryHandlerTests()
    {
        _flatRepositoryMock = new Mock<IFlatRepository>();
        var mapper = new Mapper(new MapperConfiguration(
            cfg => cfg.AddProfile<FlatMappingProfile>(), NullLoggerFactory.Instance));

        _handler = new GetAllFlatsQueryHandler(_flatRepositoryMock.Object, mapper);
    }

    [Fact]
    public async Task Handle_WithFlats_ShouldReturnPaginatedResult()
    {
        // Arrange
        var address = new Address("Długa 5", "Kraków", "30-001", "Poland");
        var flats = new List<Domain.Entities.Flat>
        {
            new("Flat 1", address),
            new("Flat 2", address)
        };
        var paginatedResult = new PaginatedResult<Domain.Entities.Flat>(flats, TotalCount: 5, Page: 1, PageSize: 2);

        _flatRepositoryMock
            .Setup(r => r.GetAllPaginatedAsync(1, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedResult);

        var query = new GetAllFlatsQuery(Page: 1, PageSize: 2);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(5);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(2);
        result.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_EmptyResult_ShouldReturnEmptyPaginatedResult()
    {
        // Arrange
        var paginatedResult = new PaginatedResult<Domain.Entities.Flat>([], TotalCount: 0, Page: 1, PageSize: 10);

        _flatRepositoryMock
            .Setup(r => r.GetAllPaginatedAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedResult);

        var query = new GetAllFlatsQuery(Page: 1, PageSize: 10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.HasNextPage.Should().BeFalse();
    }
}
