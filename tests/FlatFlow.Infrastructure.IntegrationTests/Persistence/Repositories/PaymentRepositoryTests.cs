using FluentAssertions;
using FlatFlow.Domain.Entities;
using FlatFlow.Domain.ValueObjects;
using FlatFlow.Infrastructure.Persistence;
using FlatFlow.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FlatFlow.Infrastructure.IntegrationTests.Persistence.Repositories;

public class PaymentRepositoryTests : IDisposable
{
    private readonly FlatFlowDbContext _context;
    private readonly PaymentRepository _repository;
    private readonly DbContextOptions<FlatFlowDbContext> _options;

    public PaymentRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<FlatFlowDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new FlatFlowDbContext(_options);
        _repository = new PaymentRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task GetByFlatIdPaginatedAsync_ShouldReturnPaginatedPayments()
    {
        // Arrange
        var flat = new Flat("Flat", new Address("Street", "City", "00-000", "Country"));
        var tenant = flat.AddTenant("John", "Doe", "john@test.com", "user-1");
        for (var i = 0; i < 5; i++)
        {
            flat.AddPayment($"Payment {i}", 100m + i, DateTime.UtcNow.AddDays(30), tenant.Id);
        }
        _context.Flats.Add(flat);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByFlatIdPaginatedAsync(flat.Id, page: 1, pageSize: 2);

        // Assert
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(5);
        result.HasNextPage.Should().BeTrue();
        result.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public async Task GetByFlatIdPaginatedAsync_ShouldNotIncludeOtherFlatsPayments()
    {
        // Arrange
        var flat1 = new Flat("Flat 1", new Address("Street", "City", "00-000", "Country"));
        var flat2 = new Flat("Flat 2", new Address("Street", "City", "00-000", "Country"));
        var tenant1 = flat1.AddTenant("John", "Doe", "john@test.com", "user-1");
        var tenant2 = flat2.AddTenant("Jane", "Doe", "jane@test.com", "user-2");
        flat1.AddPayment("Rent 1", 1000m, DateTime.UtcNow.AddDays(30), tenant1.Id);
        flat2.AddPayment("Rent 2", 2000m, DateTime.UtcNow.AddDays(30), tenant2.Id);
        _context.Flats.AddRange(flat1, flat2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByFlatIdPaginatedAsync(flat1.Id, page: 1, pageSize: 10);

        // Assert
        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdWithSharesAsync_ShouldIncludeShares()
    {
        // Arrange
        var flat = new Flat("Flat", new Address("Street", "City", "00-000", "Country"));
        var tenant1 = flat.AddTenant("John", "Doe", "john@test.com", "user-1");
        var tenant2 = flat.AddTenant("Jane", "Doe", "jane@test.com", "user-2");
        var payment = flat.AddPayment("Rent", 1000m, DateTime.UtcNow.AddDays(30), tenant1.Id);
        payment.AddShare(tenant1.Id, 500m);
        payment.AddShare(tenant2.Id, 500m);
        _context.Flats.Add(flat);
        await _context.SaveChangesAsync();

        // Act
        using var freshContext = new FlatFlowDbContext(_options);
        var freshRepository = new PaymentRepository(freshContext);
        var result = await freshRepository.GetByIdWithSharesAsync(payment.Id);

        // Assert
        result.Should().NotBeNull();
        result!.PaymentShares.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdWithSharesAsync_WhenNotExists_ShouldReturnNull()
    {
        // Arrange & Act
        var result = await _repository.GetByIdWithSharesAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }
}
