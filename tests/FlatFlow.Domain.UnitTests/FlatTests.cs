using FlatFlow.Domain.Entities;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;

namespace FlatFlow.Domain.UnitTests
{
    public class FlatTests
    {
        private readonly Address _validAddress = new("Main St 1", "Warsaw", "00-001", "Poland");

        [Theory]
        [InlineData("My Flat")]
        [InlineData("Apartment 42")]
        [InlineData("A")]
        public void Constructor_WithValidName_SetsName(string name)
        {
            // Arrange & Act
            var flat = new Flat(name, _validAddress);

            // Assert
            flat.Name.Should().Be(name);
        }

        [Fact]
        public void Constructor_WithValidAddress_SetsAddress()
        {
            // Arrange & Act
            var flat = new Flat("My Flat", _validAddress);

            // Assert
            flat.Address.Should().Be(_validAddress);
        }

        [Fact]
        public void Constructor_WhenCalled_GeneratesNonEmptyId()
        {
            // Arrange & Act
            var flat = new Flat("My Flat", _validAddress);

            // Assert
            flat.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void Constructor_WhenCalled_SetsCreatedAtToUtcNow()
        {
            // Arrange
            var before = DateTime.UtcNow;

            // Act
            var flat = new Flat("My Flat", _validAddress);
            var after = DateTime.UtcNow;

            // Assert
            flat.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Fact]
        public void Constructor_WhenCalled_GeneratesAccessCodeWith8UppercaseChars()
        {
            // Arrange & Act
            var flat = new Flat("My Flat", _validAddress);

            // Assert
            flat.AccessCode.Should().HaveLength(8).And.BeUpperCased();
        }

        [Fact]
        public void Constructor_WhenCalled_InitializesEmptyCollections()
        {
            // Arrange & Act
            var flat = new Flat("My Flat", _validAddress);

            // Assert
            flat.Tenants.Should().BeEmpty();
            flat.Chores.Should().BeEmpty();
            flat.Notes.Should().BeEmpty();
            flat.Payments.Should().BeEmpty();
        }

        [Fact]
        public void Constructor_WhenCalledTwice_GeneratesUniqueIds()
        {
            // Arrange & Act
            var flat1 = new Flat("Flat 1", _validAddress);
            var flat2 = new Flat("Flat 2", _validAddress);

            // Assert
            flat1.Id.Should().NotBe(flat2.Id);
        }

        [Fact]
        public void Constructor_WhenCalledTwice_GeneratesUniqueAccessCodes()
        {
            // Arrange & Act
            var flat1 = new Flat("Flat 1", _validAddress);
            var flat2 = new Flat("Flat 2", _validAddress);

            // Assert
            flat1.AccessCode.Should().NotBe(flat2.AccessCode);
        }

        [Theory]
        [InlineData("New Name")]
        [InlineData("Updated Flat")]
        [InlineData("")]
        public void UpdateName_WithValue_ChangesName(string newName)
        {
            // Arrange
            var flat = new Flat("Original", _validAddress);

            // Act
            flat.UpdateName(newName);

            // Assert
            flat.Name.Should().Be(newName);
        }

        [Fact]
        public void UpdateAddress_WithNewAddress_ChangesAddress()
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);
            var newAddress = new Address("New St 5", "Krakow", "30-001", "Poland");

            // Act
            flat.UpdateAddress(newAddress);

            // Assert
            flat.Address.Should().Be(newAddress);
        }

        [Fact]
        public void RefreshAccessCode_WhenCalled_ChangesAccessCode()
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);
            var originalCode = flat.AccessCode;

            // Act
            flat.RefreshAccessCode();

            // Assert
            flat.AccessCode.Should().NotBe(originalCode);
        }

        [Fact]
        public void RefreshAccessCode_WhenCalled_GeneratesValidAccessCode()
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);

            // Act
            flat.RefreshAccessCode();

            // Assert
            flat.AccessCode.Should().HaveLength(8).And.BeUpperCased();
        }
    }
}
