using FlatFlow.Application.Features.Tenant.Commands.AddTenant;
using FluentAssertions;

namespace FlatFlow.Application.UnitTests.Features.Tenant.Commands;

public class AddTenantCommandValidatorTests
{
    private readonly AddTenantCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new AddTenantCommand(Guid.NewGuid(), "Jan", "Kowalski", "jan@test.com", "user-1");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyFlatId_ShouldHaveError()
    {
        // Arrange
        var command = new AddTenantCommand(Guid.Empty, "Jan", "Kowalski", "jan@test.com", "user-1");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "FlatId");
    }

    [Theory]
    [InlineData("", "Kowalski", "jan@test.com", "user-1", "FirstName")]
    [InlineData("Jan", "", "jan@test.com", "user-1", "LastName")]
    [InlineData("Jan", "Kowalski", "", "user-1", "Email")]
    [InlineData("Jan", "Kowalski", "jan@test.com", "", "UserId")]
    public async Task Validate_EmptyField_ShouldHaveError(
        string firstName, string lastName, string email, string userId, string expectedProperty)
    {
        // Arrange
        var command = new AddTenantCommand(Guid.NewGuid(), firstName, lastName, email, userId);

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
        var command = new AddTenantCommand(Guid.NewGuid(), "Jan", "Kowalski", "not-an-email", "user-1");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Email");
    }

    [Fact]
    public async Task Validate_FirstNameExceedsMaxLength_ShouldHaveError()
    {
        // Arrange
        var command = new AddTenantCommand(Guid.NewGuid(), new string('a', 101), "Kowalski", "jan@test.com", "user-1");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "FirstName");
    }
}
