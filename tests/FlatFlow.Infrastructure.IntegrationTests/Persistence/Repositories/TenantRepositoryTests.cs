using FluentAssertions;
using FlatFlow.Domain.Entities;
using FlatFlow.Domain.ValueObjects;
using FlatFlow.Infrastructure.Persistence;
using FlatFlow.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FlatFlow.Infrastructure.IntegrationTests.Persistence.Repositories;

public class TenantRepositoryTests : IDisposable
{
    private readonly FlatFlowDbContext _context;
    private readonly TenantRepository _repository;

    public TenantRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<FlatFlowDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new FlatFlowDbContext(options);
        _repository = new TenantRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task GetByFlatIdAsync_ShouldReturnTenantsForFlat()
    {
        // Arrange
        var flat1 = new Flat("Flat 1", new Address("Street", "City", "00-000", "Country"));
        var flat2 = new Flat("Flat 2", new Address("Street", "City", "00-000", "Country"));
        flat1.AddTenant("John", "Doe", "john@test.com", "user-1");
        flat1.AddTenant("Jane", "Doe", "jane@test.com", "user-2");
        flat2.AddTenant("Bob", "Smith", "bob@test.com", "user-3");
        _context.Flats.AddRange(flat1, flat2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByFlatIdAsync(flat1.Id);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(t => t.FlatId.Should().Be(flat1.Id));
    }

    [Fact]
    public async Task GetByFlatIdAsync_WhenNoTenants_ShouldReturnEmptyList()
    {
        // Arrange
        var flat = new Flat("Flat", new Address("Street", "City", "00-000", "Country"));
        _context.Flats.Add(flat);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByFlatIdAsync(flat.Id);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnTenanciesForUser()
    {
        // Arrange
        var flat1 = new Flat("Flat 1", new Address("Street", "City", "00-000", "Country"));
        var flat2 = new Flat("Flat 2", new Address("Street", "City", "00-000", "Country"));
        flat1.AddTenant("John", "Doe", "john@test.com", "user-1");
        flat2.AddTenant("John", "Doe", "john@test.com", "user-1");
        flat2.AddTenant("Jane", "Doe", "jane@test.com", "user-2");
        _context.Flats.AddRange(flat1, flat2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByUserIdAsync("user-1");

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(t => t.UserId.Should().Be("user-1"));
    }

    [Fact]
    public async Task GetByUserIdAsync_WhenUserNotFound_ShouldReturnEmptyList()
    {
        // Arrange & Act
        var result = await _repository.GetByUserIdAsync("non-existent");

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByUserIdAndFlatIdAsync_ShouldReturnTenant()
    {
        // Arrange
        var flat = new Flat("Flat 1", new Address("Street", "City", "00-000", "Country"));
        var tenant = flat.AddTenant("John", "Doe", "john@test.com", "user-1");
        _context.Flats.Add(flat);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByUserIdAndFlatIdAsync("user-1", flat.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(tenant.Id);
        result.UserId.Should().Be("user-1");
        result.FlatId.Should().Be(flat.Id);
    }

    [Fact]
    public async Task GetByUserIdAndFlatIdAsync_WhenUserNotInFlat_ShouldReturnNull()
    {
        // Arrange
        var flat = new Flat("Flat 1", new Address("Street", "City", "00-000", "Country"));
        flat.AddTenant("John", "Doe", "john@test.com", "user-1");
        _context.Flats.Add(flat);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByUserIdAndFlatIdAsync("user-2", flat.Id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByUserIdAndFlatIdAsync_WhenUserInDifferentFlat_ShouldReturnNull()
    {
        // Arrange
        var flat1 = new Flat("Flat 1", new Address("Street", "City", "00-000", "Country"));
        var flat2 = new Flat("Flat 2", new Address("Street", "City", "00-000", "Country"));
        flat1.AddTenant("John", "Doe", "john@test.com", "user-1");
        _context.Flats.AddRange(flat1, flat2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByUserIdAndFlatIdAsync("user-1", flat2.Id);

        // Assert
        result.Should().BeNull();
    }
}
