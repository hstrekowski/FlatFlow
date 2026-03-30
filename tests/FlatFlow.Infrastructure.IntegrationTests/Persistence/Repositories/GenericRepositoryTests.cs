using FluentAssertions;
using FlatFlow.Domain.Entities;
using FlatFlow.Domain.ValueObjects;
using FlatFlow.Infrastructure.Persistence;
using FlatFlow.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FlatFlow.Infrastructure.IntegrationTests.Persistence.Repositories;

public class GenericRepositoryTests : IDisposable
{
    private readonly FlatFlowDbContext _context;
    private readonly GenericRepository<Flat> _repository;

    public GenericRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<FlatFlowDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new FlatFlowDbContext(options);
        _repository = new GenericRepository<Flat>(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task AddAsync_ShouldAddEntityAndReturnIt()
    {
        // Arrange
        var flat = new Flat("Test Flat", new Address("Street", "City", "00-000", "Country"));

        // Act
        var result = await _repository.AddAsync(flat);

        // Assert
        result.Should().BeSameAs(flat);
        var saved = await _context.Flats.FindAsync(flat.Id);
        saved.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityExists_ShouldReturnEntity()
    {
        // Arrange
        var flat = new Flat("Test Flat", new Address("Street", "City", "00-000", "Country"));
        _context.Flats.Add(flat);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(flat.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(flat.Id);
        result.Name.Should().Be("Test Flat");
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        // Arrange
        var flat1 = new Flat("Flat 1", new Address("Street 1", "City", "00-000", "Country"));
        var flat2 = new Flat("Flat 2", new Address("Street 2", "City", "00-000", "Country"));
        _context.Flats.AddRange(flat1, flat2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateAsync_ShouldPersistChanges()
    {
        // Arrange
        var flat = new Flat("Test Flat", new Address("Street", "City", "00-000", "Country"));
        _context.Flats.Add(flat);
        await _context.SaveChangesAsync();

        // Act
        flat.UpdateName("Updated Flat");
        await _repository.UpdateAsync(flat);

        // Assert
        var saved = await _context.Flats.FindAsync(flat.Id);
        saved!.Name.Should().Be("Updated Flat");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveEntity()
    {
        // Arrange
        var flat = new Flat("Test Flat", new Address("Street", "City", "00-000", "Country"));
        _context.Flats.Add(flat);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(flat);

        // Assert
        var saved = await _context.Flats.FindAsync(flat.Id);
        saved.Should().BeNull();
    }
}
