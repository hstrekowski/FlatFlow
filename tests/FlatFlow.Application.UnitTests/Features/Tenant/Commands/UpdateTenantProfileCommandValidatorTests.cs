using FlatFlow.Application.Features.Tenant.Commands.UpdateTenantProfile;
using FluentAssertions;

namespace FlatFlow.Application.UnitTests.Features.Tenant.Commands;

public class UpdateTenantProfileCommandValidatorTests
{
    private readonly UpdateTenantProfileCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new UpdateTenantProfileCommand(Guid.NewGuid(), "Jan", "Kowalski");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyTenantId_ShouldHaveError()
    {
        // Arrange
        var command = new UpdateTenantProfileCommand(Guid.Empty, "Jan", "Kowalski");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "TenantId");
    }

    [Theory]
    [InlineData("", "Kowalski", "FirstName")]
    [InlineData("Jan", "", "LastName")]
    public async Task Validate_EmptyField_ShouldHaveError(
        string firstName, string lastName, string expectedProperty)
    {
        // Arrange
        var command = new UpdateTenantProfileCommand(Guid.NewGuid(), firstName, lastName);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == expectedProperty);
    }

    [Fact]
    public async Task Validate_FirstNameExceedsMaxLength_ShouldHaveError()
    {
        // Arrange
        var command = new UpdateTenantProfileCommand(Guid.NewGuid(), new string('a', 101), "Kowalski");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "FirstName");
    }
}
