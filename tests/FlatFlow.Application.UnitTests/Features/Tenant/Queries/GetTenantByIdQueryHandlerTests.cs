using AutoMapper;
using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Common.Mappings;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Tenant.Queries.GetTenantById;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace FlatFlow.Application.UnitTests.Features.Tenant.Queries;

public class GetTenantByIdQueryHandlerTests
{
    private readonly Mock<ITenantRepository> _tenantRepositoryMock;
    private readonly GetTenantByIdQueryHandler _handler;

    public GetTenantByIdQueryHandlerTests()
    {
        _tenantRepositoryMock = new Mock<ITenantRepository>();
        var mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<TenantMappingProfile>();
        }, NullLoggerFactory.Instance));

        _handler = new GetTenantByIdQueryHandler(_tenantRepositoryMock.Object, mapper);
    }

    [Fact]
    public async Task Handle_ExistingTenant_ShouldReturnTenantDto()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var tenant = flat.AddTenant("Jan", "Kowalski", "jan@test.com", "user-1", true);
        _tenantRepositoryMock
            .Setup(r => r.GetByIdAsync(tenant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenant);

        var query = new GetTenantByIdQuery(tenant.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Id.Should().Be(tenant.Id);
        result.FirstName.Should().Be("Jan");
        result.LastName.Should().Be("Kowalski");
        result.Email.Should().Be("jan@test.com");
        result.IsOwner.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_NonExistingTenant_ShouldThrowNotFoundException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        _tenantRepositoryMock
            .Setup(r => r.GetByIdAsync(tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Tenant?)null);

        var query = new GetTenantByIdQuery(tenantId);

        // Act
        var act = () => _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
