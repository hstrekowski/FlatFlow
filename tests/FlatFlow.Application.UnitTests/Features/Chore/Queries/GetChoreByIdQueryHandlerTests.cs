using AutoMapper;
using FlatFlow.Application.Common.Exceptions;
using FlatFlow.Application.Common.Mappings;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Chore.Queries.GetChoreById;
using FlatFlow.Domain.Enums;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace FlatFlow.Application.UnitTests.Features.Chore.Queries;

public class GetChoreByIdQueryHandlerTests
{
    private readonly Mock<IChoreRepository> _choreRepositoryMock;
    private readonly GetChoreByIdQueryHandler _handler;

    public GetChoreByIdQueryHandlerTests()
    {
        _choreRepositoryMock = new Mock<IChoreRepository>();
        var mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ChoreMappingProfile>();
        }, NullLoggerFactory.Instance));

        _handler = new GetChoreByIdQueryHandler(_choreRepositoryMock.Object, mapper);
    }

    [Fact]
    public async Task Handle_ExistingChore_ShouldReturnChoreDetailDto()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var chore = flat.AddChore("Sprzątanie", "Opis", ChoreFrequency.Weekly, Guid.NewGuid());
        var tenantId = Guid.NewGuid();
        var dueDate = DateTime.UtcNow.AddDays(7);
        chore.AddAssignment(tenantId, dueDate);

        _choreRepositoryMock
            .Setup(r => r.GetByIdWithAssignmentsAsync(chore.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(chore);

        var query = new GetChoreByIdQuery(chore.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Id.Should().Be(chore.Id);
        result.Title.Should().Be("Sprzątanie");
        result.Description.Should().Be("Opis");
        result.Frequency.Should().Be(ChoreFrequency.Weekly);
        result.Assignments.Should().ContainSingle();
        result.Assignments[0].TenantId.Should().Be(tenantId);
        result.Assignments[0].DueDate.Should().Be(dueDate);
        result.Assignments[0].IsCompleted.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_NonExistingChore_ShouldThrowNotFoundException()
    {
        // Arrange
        var choreId = Guid.NewGuid();
        _choreRepositoryMock
            .Setup(r => r.GetByIdWithAssignmentsAsync(choreId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Chore?)null);

        var query = new GetChoreByIdQuery(choreId);

        // Act
        var act = () => _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
