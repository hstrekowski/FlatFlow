using AutoMapper;
using FlatFlow.Application.Common.Mappings;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Tenant.Queries.GetTenantsByFlatId;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace FlatFlow.Application.UnitTests.Features.Tenant.Queries;

public class GetTenantsByFlatIdQueryHandlerTests
{
    private readonly Mock<ITenantRepository> _tenantRepositoryMock;
    private readonly GetTenantsByFlatIdQueryHandler _handler;

    public GetTenantsByFlatIdQueryHandlerTests()
    {
        _tenantRepositoryMock = new Mock<ITenantRepository>();
        var mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<TenantMappingProfile>();
        }, NullLoggerFactory.Instance));

        _handler = new GetTenantsByFlatIdQueryHandler(_tenantRepositoryMock.Object, mapper);
    }

    [Fact]
    public async Task Handle_ExistingFlat_ShouldReturnTenantDtos()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var tenant1 = flat.AddTenant("Jan", "Kowalski", "jan@test.com", "user-1");
        var tenant2 = flat.AddTenant("Anna", "Nowak", "anna@test.com", "user-2");

        _tenantRepositoryMock
            .Setup(r => r.GetByFlatIdAsync(flat.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync([tenant1, tenant2]);

        var query = new GetTenantsByFlatIdQuery(flat.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(t => t.FirstName == "Jan" && t.LastName == "Kowalski");
        result.Should().Contain(t => t.FirstName == "Anna" && t.LastName == "Nowak");
    }

    [Fact]
    public async Task Handle_NoTenants_ShouldReturnEmptyList()
    {
        // Arrange
        var flatId = Guid.NewGuid();
        _tenantRepositoryMock
            .Setup(r => r.GetByFlatIdAsync(flatId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var query = new GetTenantsByFlatIdQuery(flatId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}
