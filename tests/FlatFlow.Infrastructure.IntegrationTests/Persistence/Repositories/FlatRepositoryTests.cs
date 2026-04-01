using FluentAssertions;
using FlatFlow.Domain.Entities;
using FlatFlow.Domain.Enums;
using FlatFlow.Domain.ValueObjects;
using FlatFlow.Infrastructure.Persistence;
using FlatFlow.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FlatFlow.Infrastructure.IntegrationTests.Persistence.Repositories;

public class FlatRepositoryTests : IDisposable
{
    private readonly FlatFlowDbContext _context;
    private readonly FlatRepository _repository;
    private readonly DbContextOptions<FlatFlowDbContext> _options;

    public FlatRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<FlatFlowDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new FlatFlowDbContext(_options);
        _repository = new FlatRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    private Flat CreateFlat(string name = "Test Flat")
    {
        return new Flat(name, new Address("Street", "City", "00-000", "Country"));
    }

    [Fact]
    public async Task GetByAccessCodeAsync_WhenExists_ShouldReturnFlat()
    {
        // Arrange
        var flat = CreateFlat();
        _context.Flats.Add(flat);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByAccessCodeAsync(flat.AccessCode);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(flat.Id);
    }

    [Fact]
    public async Task GetByAccessCodeAsync_WhenNotExists_ShouldReturnNull()
    {
        // Arrange & Act
        var result = await _repository.GetByAccessCodeAsync("NONEXIST");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByAccessCodeWithTenantsAsync_ShouldIncludeTenants()
    {
        // Arrange
        var flat = CreateFlat();
        flat.AddTenant("John", "Doe", "john@test.com", "user-1", true);
        _context.Flats.Add(flat);
        await _context.SaveChangesAsync();

        // Act
        using var freshContext = new FlatFlowDbContext(_options);
        var freshRepository = new FlatRepository(freshContext);
        var result = await freshRepository.GetByAccessCodeWithTenantsAsync(flat.AccessCode);

        // Assert
        result.Should().NotBeNull();
        result!.Tenants.Should().HaveCount(1);
        result.Tenants[0].FirstName.Should().Be("John");
    }

    [Fact]
    public async Task GetByTenantUserIdAsync_ShouldReturnFlatsForUser()
    {
        // Arrange
        var flat1 = CreateFlat("Flat 1");
        var flat2 = CreateFlat("Flat 2");
        var flat3 = CreateFlat("Flat 3");
        flat1.AddTenant("John", "Doe", "john@test.com", "user-1");
        flat2.AddTenant("John", "Doe", "john@test.com", "user-1");
        flat3.AddTenant("Jane", "Doe", "jane@test.com", "user-2");
        _context.Flats.AddRange(flat1, flat2, flat3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByTenantUserIdAsync("user-1");

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdWithTenantsAsync_ShouldIncludeTenants()
    {
        // Arrange
        var flat = CreateFlat();
        flat.AddTenant("John", "Doe", "john@test.com", "user-1");
        flat.AddTenant("Jane", "Doe", "jane@test.com", "user-2");
        _context.Flats.Add(flat);
        await _context.SaveChangesAsync();

        // Act
        using var freshContext = new FlatFlowDbContext(_options);
        var freshRepository = new FlatRepository(freshContext);
        var result = await freshRepository.GetByIdWithTenantsAsync(flat.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Tenants.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdWithChoresAsync_ShouldIncludeChores()
    {
        // Arrange
        var flat = CreateFlat();
        var tenant = flat.AddTenant("John", "Doe", "john@test.com", "user-1");
        flat.AddChore("Clean kitchen", "Wipe counters", ChoreFrequency.Weekly, tenant.Id);
        _context.Flats.Add(flat);
        await _context.SaveChangesAsync();

        // Act
        using var freshContext = new FlatFlowDbContext(_options);
        var freshRepository = new FlatRepository(freshContext);
        var result = await freshRepository.GetByIdWithChoresAsync(flat.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Chores.Should().HaveCount(1);
        result.Chores[0].Title.Should().Be("Clean kitchen");
    }

    [Fact]
    public async Task GetByIdWithPaymentsAsync_ShouldIncludePayments()
    {
        // Arrange
        var flat = CreateFlat();
        var tenant = flat.AddTenant("John", "Doe", "john@test.com", "user-1");
        flat.AddPayment("Rent", 1000m, DateTime.UtcNow.AddDays(30), tenant.Id);
        _context.Flats.Add(flat);
        await _context.SaveChangesAsync();

        // Act
        using var freshContext = new FlatFlowDbContext(_options);
        var freshRepository = new FlatRepository(freshContext);
        var result = await freshRepository.GetByIdWithPaymentsAsync(flat.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Payments.Should().HaveCount(1);
        result.Payments[0].Title.Should().Be("Rent");
    }

    [Fact]
    public async Task GetByIdWithNotesAsync_ShouldIncludeNotes()
    {
        // Arrange
        var flat = CreateFlat();
        var tenant = flat.AddTenant("John", "Doe", "john@test.com", "user-1");
        flat.AddNote("WiFi password", "pass123", tenant.Id);
        _context.Flats.Add(flat);
        await _context.SaveChangesAsync();

        // Act
        using var freshContext = new FlatFlowDbContext(_options);
        var freshRepository = new FlatRepository(freshContext);
        var result = await freshRepository.GetByIdWithNotesAsync(flat.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Notes.Should().HaveCount(1);
        result.Notes[0].Title.Should().Be("WiFi password");
    }

    [Fact]
    public async Task GetByIdWithAllAsync_ShouldIncludeAllRelations()
    {
        // Arrange
        var flat = CreateFlat();
        var tenant = flat.AddTenant("John", "Doe", "john@test.com", "user-1");
        var chore = flat.AddChore("Clean", "Description", ChoreFrequency.Daily, tenant.Id);
        chore.AddAssignment(tenant.Id, DateTime.UtcNow.AddDays(1));
        var payment = flat.AddPayment("Rent", 1000m, DateTime.UtcNow.AddDays(30), tenant.Id);
        payment.AddShare(tenant.Id, 500m);
        flat.AddNote("Note", "Content", tenant.Id);
        _context.Flats.Add(flat);
        await _context.SaveChangesAsync();

        // Act
        using var freshContext = new FlatFlowDbContext(_options);
        var freshRepository = new FlatRepository(freshContext);
        var result = await freshRepository.GetByIdWithAllAsync(flat.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Tenants.Should().HaveCount(1);
        result.Chores.Should().HaveCount(1);
        result.Chores[0].ChoreAssignments.Should().HaveCount(1);
        result.Payments.Should().HaveCount(1);
        result.Payments[0].PaymentShares.Should().HaveCount(1);
        result.Notes.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAllPaginatedAsync_ShouldReturnCorrectPage()
    {
        // Arrange
        for (var i = 0; i < 5; i++)
        {
            _context.Flats.Add(CreateFlat($"Flat {i}"));
        }
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllPaginatedAsync(page: 1, pageSize: 2);

        // Assert
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(5);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(2);
        result.HasNextPage.Should().BeTrue();
        result.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllPaginatedAsync_SecondPage_ShouldHavePreviousPage()
    {
        // Arrange
        for (var i = 0; i < 5; i++)
        {
            _context.Flats.Add(CreateFlat($"Flat {i}"));
        }
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllPaginatedAsync(page: 2, pageSize: 2);

        // Assert
        result.Items.Should().HaveCount(2);
        result.HasPreviousPage.Should().BeTrue();
        result.HasNextPage.Should().BeTrue();
    }
}
