using AutoMapper;
using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Common.Mappings;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Flat.Queries.GetFlatByAccessCode;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace FlatFlow.Application.UnitTests.Features.Flat.Queries;

public class GetFlatByAccessCodeQueryHandlerTests
{
    private readonly Mock<IFlatRepository> _flatRepositoryMock;
    private readonly GetFlatByAccessCodeQueryHandler _handler;

    public GetFlatByAccessCodeQueryHandlerTests()
    {
        _flatRepositoryMock = new Mock<IFlatRepository>();
        var mapper = new Mapper(new MapperConfiguration(
            cfg => cfg.AddProfile<FlatMappingProfile>(), NullLoggerFactory.Instance));

        _handler = new GetFlatByAccessCodeQueryHandler(_flatRepositoryMock.Object, mapper);
    }

    [Fact]
    public async Task Handle_ExistingAccessCode_ShouldReturnFlatDto()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        _flatRepositoryMock
            .Setup(r => r.GetByAccessCodeAsync(flat.AccessCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(flat);

        var query = new GetFlatByAccessCodeQuery(flat.AccessCode);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Id.Should().Be(flat.Id);
        result.Name.Should().Be("Mieszkanie");
        result.City.Should().Be("Kraków");
        result.AccessCode.Should().Be(flat.AccessCode);
    }

    [Fact]
    public async Task Handle_NonExistingAccessCode_ShouldThrowNotFoundException()
    {
        // Arrange
        _flatRepositoryMock
            .Setup(r => r.GetByAccessCodeAsync("INVALID", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Flat?)null);

        var query = new GetFlatByAccessCodeQuery("INVALID");

        // Act
        var act = () => _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
