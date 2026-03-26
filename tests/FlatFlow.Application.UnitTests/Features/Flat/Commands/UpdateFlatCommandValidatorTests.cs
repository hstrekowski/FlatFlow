using FlatFlow.Application.Features.Flat.Commands.UpdateFlat;
using FluentAssertions;

namespace FlatFlow.Application.UnitTests.Features.Flat.Commands;

public class UpdateFlatCommandValidatorTests
{
    private readonly UpdateFlatCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new UpdateFlatCommand(Guid.NewGuid(), "Mieszkanie", "Długa 5", "Kraków", "30-001", "Poland");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyFlatId_ShouldHaveError()
    {
        // Arrange
        var command = new UpdateFlatCommand(Guid.Empty, "Mieszkanie", "Długa 5", "Kraków", "30-001", "Poland");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "FlatId");
    }

    [Theory]
    [InlineData("", "Długa 5", "Kraków", "30-001", "Poland", "Name")]
    [InlineData("Mieszkanie", "", "Kraków", "30-001", "Poland", "Street")]
    [InlineData("Mieszkanie", "Długa 5", "", "30-001", "Poland", "City")]
    [InlineData("Mieszkanie", "Długa 5", "Kraków", "", "Poland", "ZipCode")]
    [InlineData("Mieszkanie", "Długa 5", "Kraków", "30-001", "", "Country")]
    public async Task Validate_EmptyField_ShouldHaveError(
        string name, string street, string city, string zipCode, string country, string expectedProperty)
    {
        // Arrange
        var command = new UpdateFlatCommand(Guid.NewGuid(), name, street, city, zipCode, country);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == expectedProperty);
    }
}
