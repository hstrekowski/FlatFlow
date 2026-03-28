using FlatFlow.Application.Features.Tenant.Commands.JoinFlat;
using FluentAssertions;

namespace FlatFlow.Application.UnitTests.Features.Tenant.Commands;

public class JoinFlatCommandValidatorTests
{
    private readonly JoinFlatCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new JoinFlatCommand("ABC123", "Jan", "Kowalski", "jan@test.com", "user-1");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "Jan", "Kowalski", "jan@test.com", "user-1", "AccessCode")]
    [InlineData("ABC123", "", "Kowalski", "jan@test.com", "user-1", "FirstName")]
    [InlineData("ABC123", "Jan", "", "jan@test.com", "user-1", "LastName")]
    [InlineData("ABC123", "Jan", "Kowalski", "", "user-1", "Email")]
    [InlineData("ABC123", "Jan", "Kowalski", "jan@test.com", "", "UserId")]
    public async Task Validate_EmptyField_ShouldHaveError(
        string accessCode, string firstName, string lastName, string email, string userId, string expectedProperty)
    {
        // Arrange
        var command = new JoinFlatCommand(accessCode, firstName, lastName, email, userId);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == expectedProperty);
    }

    [Fact]
    public async Task Validate_InvalidEmail_ShouldHaveError()
    {
        // Arrange
        var command = new JoinFlatCommand("ABC123", "Jan", "Kowalski", "not-an-email", "user-1");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Email");
    }
}
