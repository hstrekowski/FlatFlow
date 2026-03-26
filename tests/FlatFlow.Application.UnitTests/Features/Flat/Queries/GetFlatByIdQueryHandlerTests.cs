using AutoMapper;
using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Common.Mappings;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Flat.Queries.GetFlatById;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace FlatFlow.Application.UnitTests.Features.Flat.Queries;

public class GetFlatByIdQueryHandlerTests
{
    private readonly Mock<IFlatRepository> _flatRepositoryMock;
    private readonly GetFlatByIdQueryHandler _handler;

    public GetFlatByIdQueryHandlerTests()
    {
        _flatRepositoryMock = new Mock<IFlatRepository>();
        var mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<FlatMappingProfile>();
            cfg.AddProfile<TenantMappingProfile>();
            cfg.AddProfile<ChoreMappingProfile>();
            cfg.AddProfile<PaymentMappingProfile>();
            cfg.AddProfile<NoteMappingProfile>();
        }, NullLoggerFactory.Instance));

        _handler = new GetFlatByIdQueryHandler(_flatRepositoryMock.Object, mapper);
    }

    [Fact]
    public async Task Handle_ExistingFlat_ShouldReturnFlatDetailDto()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        _flatRepositoryMock
            .Setup(r => r.GetByIdWithAllAsync(flat.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(flat);

        var query = new GetFlatByIdQuery(flat.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Id.Should().Be(flat.Id);
        result.Name.Should().Be("Mieszkanie");
        result.Street.Should().Be("Długa 5");
        result.City.Should().Be("Kraków");
    }

    [Fact]
    public async Task Handle_NonExistingFlat_ShouldThrowNotFoundException()
    {
        // Arrange
        var flatId = Guid.NewGuid();
        _flatRepositoryMock
            .Setup(r => r.GetByIdWithAllAsync(flatId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Flat?)null);

        var query = new GetFlatByIdQuery(flatId);

        // Act
        var act = () => _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
