using FlatFlow.Application.Common.Models;
using FluentAssertions;

namespace FlatFlow.Application.UnitTests.Common.Models;

public class PaginatedResultTests
{
    [Fact]
    public void HasPreviousPage_FirstPage_ShouldBeFalse()
    {
        // Arrange
        var result = new PaginatedResult<string>(["a", "b"], TotalCount: 10, Page: 1, PageSize: 2);

        // Act & Assert
        result.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public void HasPreviousPage_SecondPage_ShouldBeTrue()
    {
        // Arrange
        var result = new PaginatedResult<string>(["c", "d"], TotalCount: 10, Page: 2, PageSize: 2);

        // Act & Assert
        result.HasPreviousPage.Should().BeTrue();
    }

    [Fact]
    public void HasNextPage_MoreItemsExist_ShouldBeTrue()
    {
        // Arrange
        var result = new PaginatedResult<string>(["a", "b"], TotalCount: 10, Page: 1, PageSize: 2);

        // Act & Assert
        result.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public void HasNextPage_LastPage_ShouldBeFalse()
    {
        // Arrange
        var result = new PaginatedResult<string>(["i", "j"], TotalCount: 10, Page: 5, PageSize: 2);

        // Act & Assert
        result.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public void HasNextPage_ExactlyOnePage_ShouldBeFalse()
    {
        // Arrange
        var result = new PaginatedResult<string>(["a", "b", "c"], TotalCount: 3, Page: 1, PageSize: 3);

        // Act & Assert
        result.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public void HasNextPage_EmptyResult_ShouldBeFalse()
    {
        // Arrange
        var result = new PaginatedResult<string>([], TotalCount: 0, Page: 1, PageSize: 10);

        // Act & Assert
        result.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public void Properties_ShouldReturnCorrectValues()
    {
        // Arrange
        var items = new List<string> { "a", "b" };

        // Act
        var result = new PaginatedResult<string>(items, TotalCount: 50, Page: 3, PageSize: 2);

        // Assert
        result.Items.Should().BeEquivalentTo(items);
        result.TotalCount.Should().Be(50);
        result.Page.Should().Be(3);
        result.PageSize.Should().Be(2);
    }
}
