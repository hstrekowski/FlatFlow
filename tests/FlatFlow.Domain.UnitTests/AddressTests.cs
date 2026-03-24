using FlatFlow.Domain.ValueObjects;
using FluentAssertions;

namespace FlatFlow.Domain.UnitTests
{
    public class AddressTests
    {
        [Fact]
        public void Constructor_WithValidValues_SetsProperties()
        {
            // Arrange & Act
            var address = new Address("Main St 1", "Warsaw", "00-001", "Poland");

            // Assert
            address.Street.Should().Be("Main St 1");
            address.City.Should().Be("Warsaw");
            address.ZipCode.Should().Be("00-001");
            address.Country.Should().Be("Poland");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidStreet_ThrowsArgumentException(string? street)
        {
            // Arrange & Act
            var act = () => new Address(street!, "Warsaw", "00-001", "Poland");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Street cannot be empty.*");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidCity_ThrowsArgumentException(string? city)
        {
            // Arrange & Act
            var act = () => new Address("Main St 1", city!, "00-001", "Poland");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("City cannot be empty.*");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidZipCode_ThrowsArgumentException(string? zipCode)
        {
            // Arrange & Act
            var act = () => new Address("Main St 1", "Warsaw", zipCode!, "Poland");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Zip code cannot be empty.*");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidCountry_ThrowsArgumentException(string? country)
        {
            // Arrange & Act
            var act = () => new Address("Main St 1", "Warsaw", "00-001", country!);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Country cannot be empty.*");
        }

        [Fact]
        public void Equality_TwoAddressesWithSameValues_AreEqual()
        {
            // Arrange
            var address1 = new Address("Main St 1", "Warsaw", "00-001", "Poland");
            var address2 = new Address("Main St 1", "Warsaw", "00-001", "Poland");

            // Assert
            address1.Should().Be(address2);
        }

        [Fact]
        public void Equality_TwoAddressesWithDifferentValues_AreNotEqual()
        {
            // Arrange
            var address1 = new Address("Main St 1", "Warsaw", "00-001", "Poland");
            var address2 = new Address("Other St 5", "Krakow", "30-001", "Poland");

            // Assert
            address1.Should().NotBe(address2);
        }
    }
}
