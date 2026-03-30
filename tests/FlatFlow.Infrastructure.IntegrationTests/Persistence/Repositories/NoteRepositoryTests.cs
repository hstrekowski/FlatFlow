using FluentAssertions;
using FlatFlow.Domain.Entities;
using FlatFlow.Domain.ValueObjects;
using FlatFlow.Infrastructure.Persistence;
using FlatFlow.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FlatFlow.Infrastructure.IntegrationTests.Persistence.Repositories;

public class NoteRepositoryTests : IDisposable
{
    private readonly FlatFlowDbContext _context;
    private readonly NoteRepository _repository;

    public NoteRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<FlatFlowDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new FlatFlowDbContext(options);
        _repository = new NoteRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task GetByFlatIdPaginatedAsync_ShouldReturnPaginatedNotes()
    {
        // Arrange
        var flat = new Flat("Flat", new Address("Street", "City", "00-000", "Country"));
        var tenant = flat.AddTenant("John", "Doe", "john@test.com", "user-1");
        for (var i = 0; i < 5; i++)
        {
            flat.AddNote($"Note {i}", $"Content {i}", tenant.Id);
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
    public async Task GetByFlatIdPaginatedAsync_ShouldNotIncludeOtherFlatsNotes()
    {
        // Arrange
        var flat1 = new Flat("Flat 1", new Address("Street", "City", "00-000", "Country"));
        var flat2 = new Flat("Flat 2", new Address("Street", "City", "00-000", "Country"));
        var tenant1 = flat1.AddTenant("John", "Doe", "john@test.com", "user-1");
        var tenant2 = flat2.AddTenant("Jane", "Doe", "jane@test.com", "user-2");
        flat1.AddNote("Note 1", "Content", tenant1.Id);
        flat2.AddNote("Note 2", "Content", tenant2.Id);
        _context.Flats.AddRange(flat1, flat2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByFlatIdPaginatedAsync(flat1.Id, page: 1, pageSize: 10);

        // Assert
        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task GetByFlatIdPaginatedAsync_WhenNoNotes_ShouldReturnEmptyResult()
    {
        // Arrange
        var flat = new Flat("Flat", new Address("Street", "City", "00-000", "Country"));
        _context.Flats.Add(flat);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByFlatIdPaginatedAsync(flat.Id, page: 1, pageSize: 10);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}
