using AutoMapper;
using FlatFlow.Application.Common.Mappings;
using FlatFlow.Application.Common.Models;
using FlatFlow.Application.Contracts.Persistence;
using FlatFlow.Application.Features.Note.Queries.GetNotesByFlatId;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace FlatFlow.Application.UnitTests.Features.Note.Queries;

public class GetNotesByFlatIdQueryHandlerTests
{
    private readonly Mock<INoteRepository> _noteRepositoryMock;
    private readonly GetNotesByFlatIdQueryHandler _handler;

    public GetNotesByFlatIdQueryHandlerTests()
    {
        _noteRepositoryMock = new Mock<INoteRepository>();
        var mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<NoteMappingProfile>();
        }, NullLoggerFactory.Instance));

        _handler = new GetNotesByFlatIdQueryHandler(_noteRepositoryMock.Object, mapper);
    }

    [Fact]
    public async Task Handle_WithNotes_ShouldReturnPaginatedResult()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var note1 = flat.AddNote("Zakupy", "Kupić mleko", Guid.NewGuid());
        var note2 = flat.AddNote("Naprawy", "Naprawić kran", Guid.NewGuid());
        var paginatedResult = new PaginatedResult<Domain.Entities.Note>(
            [note1, note2], TotalCount: 5, Page: 1, PageSize: 2);

        _noteRepositoryMock
            .Setup(r => r.GetByFlatIdPaginatedAsync(flat.Id, 1, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedResult);

        var query = new GetNotesByFlatIdQuery(flat.Id, Page: 1, PageSize: 2);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(5);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(2);
        result.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_EmptyResult_ShouldReturnEmptyPaginatedResult()
    {
        // Arrange
        var flatId = Guid.NewGuid();
        var paginatedResult = new PaginatedResult<Domain.Entities.Note>(
            [], TotalCount: 0, Page: 1, PageSize: 10);

        _noteRepositoryMock
            .Setup(r => r.GetByFlatIdPaginatedAsync(flatId, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedResult);

        var query = new GetNotesByFlatIdQuery(flatId, Page: 1, PageSize: 10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.HasNextPage.Should().BeFalse();
    }
}
