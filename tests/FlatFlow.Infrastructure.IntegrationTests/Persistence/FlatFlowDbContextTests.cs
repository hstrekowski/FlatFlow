using FluentAssertions;
using FlatFlow.Domain.Entities;
using FlatFlow.Domain.ValueObjects;
using FlatFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlatFlow.Infrastructure.IntegrationTests.Persistence;

public class FlatFlowDbContextTests : IDisposable
{
    private readonly FlatFlowDbContext _context;

    public FlatFlowDbContextTests()
    {
        var options = new DbContextOptionsBuilder<FlatFlowDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new FlatFlowDbContext(options);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task SaveChangesAsync_WhenEntityModified_SetsUpdatedAt()
    {
        // Arrange
        var flat = new Flat("Test Flat", new Address("Street", "City", "00-000", "Country"));
        _context.Flats.Add(flat);
        await _context.SaveChangesAsync();
        flat.UpdatedAt.Should().BeNull();

        // Act
        flat.UpdateName("Updated Flat");
        _context.Entry(flat).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        // Assert
        flat.UpdatedAt.Should().NotBeNull();
        flat.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task SaveChangesAsync_WhenEntityAdded_DoesNotSetUpdatedAt()
    {
        // Arrange
        var flat = new Flat("Test Flat", new Address("Street", "City", "00-000", "Country"));

        // Act
        _context.Flats.Add(flat);
        await _context.SaveChangesAsync();

        // Assert
        flat.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public async Task DbContext_AppliesEntityConfigurations()
    {
        // Arrange
        var flat = new Flat("Test Flat", new Address("Street", "City", "00-000", "Country"));

        // Act
        _context.Flats.Add(flat);
        await _context.SaveChangesAsync();

        // Assert
        var savedFlat = await _context.Flats.FirstOrDefaultAsync(f => f.Id == flat.Id);
        savedFlat.Should().NotBeNull();
        savedFlat!.Address.Street.Should().Be("Street");
        savedFlat.Address.City.Should().Be("City");
    }
}
