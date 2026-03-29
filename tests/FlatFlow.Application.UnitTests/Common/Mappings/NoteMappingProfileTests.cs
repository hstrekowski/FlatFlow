using AutoMapper;
using FlatFlow.Application.Common.Mappings;
using FlatFlow.Application.Features.Note.Queries.DTOs;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace FlatFlow.Application.UnitTests.Common.Mappings;

public class NoteMappingProfileTests
{
    private readonly IMapper _mapper;

    public NoteMappingProfileTests()
    {
        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<NoteMappingProfile>();
        }, NullLoggerFactory.Instance));
    }

    [Fact]
    public void MappingProfile_ShouldHaveValidConfiguration()
    {
        // Arrange & Act & Assert
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    public void Map_NoteToNoteDto_ShouldMapCorrectly()
    {
        // Arrange
        var flat = new Domain.Entities.Flat("Mieszkanie", new Address("Długa 5", "Kraków", "30-001", "Poland"));
        var authorId = Guid.NewGuid();
        var note = flat.AddNote("Zakupy", "Kupić mleko", authorId);

        // Act
        var result = _mapper.Map<NoteDto>(note);

        // Assert
        result.Id.Should().Be(note.Id);
        result.Title.Should().Be("Zakupy");
        result.Content.Should().Be("Kupić mleko");
        result.AuthorId.Should().Be(authorId);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}
