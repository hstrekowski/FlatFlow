using AutoMapper;
using FlatFlow.Application.Common.Mappings;
using FlatFlow.Application.Contracts.Identity;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Flat.Queries.GetMyFlats;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace FlatFlow.Application.UnitTests.Features.Flat.Queries;

public class GetMyFlatsQueryHandlerTests
{
    private const string TestUserId = "test-user-id";
    private readonly Mock<IFlatRepository> _flatRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly GetMyFlatsQueryHandler _handler;

    public GetMyFlatsQueryHandlerTests()
    {
        _flatRepositoryMock = new Mock<IFlatRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _currentUserServiceMock.Setup(s => s.UserId).Returns(TestUserId);
        var mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<FlatMappingProfile>();
        }, NullLoggerFactory.Instance));

        _handler = new GetMyFlatsQueryHandler(
            _flatRepositoryMock.Object,
            _currentUserServiceMock.Object,
            mapper);
    }

    [Fact]
    public async Task Handle_UserHasFlats_ShouldReturnMappedList()
    {
        // Arrange
        var flat1 = new Domain.Entities.Flat("Mieszkanie 1", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var flat2 = new Domain.Entities.Flat("Mieszkanie 2", new Address("Krótka 3", "Warszawa", "00-001", "Poland"));
        _flatRepositoryMock
            .Setup(r => r.GetByTenantUserIdAsync(TestUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([flat1, flat2]);

        // Act
        var result = await _handler.Handle(new GetMyFlatsQuery(), CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Mieszkanie 1");
        result[1].Name.Should().Be("Mieszkanie 2");
    }

    [Fact]
    public async Task Handle_UserHasNoFlats_ShouldReturnEmptyList()
    {
        // Arrange
        _flatRepositoryMock
            .Setup(r => r.GetByTenantUserIdAsync(TestUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await _handler.Handle(new GetMyFlatsQuery(), CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}
