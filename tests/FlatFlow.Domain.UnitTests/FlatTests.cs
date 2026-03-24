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
            var flat = new Flat(name, _validAddress);

            flat.Name.Should().Be(name);
        }

        [Fact]
        public void Constructor_WithValidAddress_SetsAddress()
        {
            var flat = new Flat("My Flat", _validAddress);

            flat.Address.Should().Be(_validAddress);
        }

        [Fact]
        public void Constructor_WhenCalled_GeneratesNonEmptyId()
        {
            var flat = new Flat("My Flat", _validAddress);

            flat.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void Constructor_WhenCalled_SetsCreatedAtToUtcNow()
        {
            var before = DateTime.UtcNow;
            var flat = new Flat("My Flat", _validAddress);
            var after = DateTime.UtcNow;

            flat.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        }

        [Fact]
        public void Constructor_WhenCalled_GeneratesAccessCodeWith8UppercaseChars()
        {
            var flat = new Flat("My Flat", _validAddress);

            flat.AccessCode.Should().HaveLength(8).And.BeUpperCased();
        }

        [Fact]
        public void Constructor_WhenCalled_InitializesEmptyCollections()
        {
            var flat = new Flat("My Flat", _validAddress);

            flat.Tenants.Should().BeEmpty();
            flat.Chores.Should().BeEmpty();
            flat.Notes.Should().BeEmpty();
            flat.Payments.Should().BeEmpty();
        }

        [Fact]
        public void Constructor_WhenCalledTwice_GeneratesUniqueIds()
        {
            var flat1 = new Flat("Flat 1", _validAddress);
            var flat2 = new Flat("Flat 2", _validAddress);

            flat1.Id.Should().NotBe(flat2.Id);
        }

        [Fact]
        public void Constructor_WhenCalledTwice_GeneratesUniqueAccessCodes()
        {
            var flat1 = new Flat("Flat 1", _validAddress);
            var flat2 = new Flat("Flat 2", _validAddress);

            flat1.AccessCode.Should().NotBe(flat2.AccessCode);
        }

        [Theory]
        [InlineData("New Name")]
        [InlineData("Updated Flat")]
        [InlineData("")]
        public void UpdateName_WithValue_ChangesName(string newName)
        {
            var flat = new Flat("Original", _validAddress);

            flat.UpdateName(newName);

            flat.Name.Should().Be(newName);
        }

        [Fact]
        public void UpdateAddress_WithNewAddress_ChangesAddress()
        {
            var flat = new Flat("My Flat", _validAddress);
            var newAddress = new Address("New St 5", "Krakow", "30-001", "Poland");

            flat.UpdateAddress(newAddress);

            flat.Address.Should().Be(newAddress);
        }

        [Fact]
        public void RefreshAccessCode_WhenCalled_ChangesAccessCode()
        {
            var flat = new Flat("My Flat", _validAddress);
            var originalCode = flat.AccessCode;

            flat.RefreshAccessCode();

            flat.AccessCode.Should().NotBe(originalCode);
        }

        [Fact]
        public void RefreshAccessCode_WhenCalled_GeneratesValidAccessCode()
        {
            var flat = new Flat("My Flat", _validAddress);

            flat.RefreshAccessCode();

            flat.AccessCode.Should().HaveLength(8).And.BeUpperCased();
        }
    }
}
