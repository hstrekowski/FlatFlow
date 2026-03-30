using FluentAssertions;
using FlatFlow.Domain.Entities;
using FlatFlow.Domain.Enums;
using FlatFlow.Domain.ValueObjects;
using FlatFlow.Infrastructure.Persistence;
using FlatFlow.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FlatFlow.Infrastructure.IntegrationTests.Persistence.Repositories;

public class ChoreRepositoryTests : IDisposable
{
    private readonly FlatFlowDbContext _context;
    private readonly ChoreRepository _repository;
    private readonly DbContextOptions<FlatFlowDbContext> _options;

    public ChoreRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<FlatFlowDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new FlatFlowDbContext(_options);
        _repository = new ChoreRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task GetByFlatIdAsync_ShouldReturnChoresForFlat()
    {
        // Arrange
        var flat1 = new Flat("Flat 1", new Address("Street", "City", "00-000", "Country"));
        var flat2 = new Flat("Flat 2", new Address("Street", "City", "00-000", "Country"));
        flat1.AddChore("Clean kitchen", "Wipe counters", ChoreFrequency.Weekly);
        flat1.AddChore("Vacuum", "All rooms", ChoreFrequency.Daily);
        flat2.AddChore("Mow lawn", "", ChoreFrequency.Monthly);
        _context.Flats.AddRange(flat1, flat2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByFlatIdAsync(flat1.Id);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(c => c.FlatId.Should().Be(flat1.Id));
    }

    [Fact]
    public async Task GetByFlatIdAsync_WhenNoChores_ShouldReturnEmptyList()
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
    public async Task GetByIdWithAssignmentsAsync_ShouldIncludeAssignments()
    {
        // Arrange
        var flat = new Flat("Flat", new Address("Street", "City", "00-000", "Country"));
        var tenant = flat.AddTenant("John", "Doe", "john@test.com", "user-1");
        var chore = flat.AddChore("Clean", "Description", ChoreFrequency.Weekly);
        chore.AddAssignment(tenant.Id, DateTime.UtcNow.AddDays(1));
        _context.Flats.Add(flat);
        await _context.SaveChangesAsync();

        // Act
        using var freshContext = new FlatFlowDbContext(_options);
        var freshRepository = new ChoreRepository(freshContext);
        var result = await freshRepository.GetByIdWithAssignmentsAsync(chore.Id);

        // Assert
        result.Should().NotBeNull();
        result!.ChoreAssignments.Should().HaveCount(1);
        result.ChoreAssignments[0].TenantId.Should().Be(tenant.Id);
    }

    [Fact]
    public async Task GetByIdWithAssignmentsAsync_WhenNotExists_ShouldReturnNull()
    {
        // Arrange & Act
        var result = await _repository.GetByIdWithAssignmentsAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }
}
