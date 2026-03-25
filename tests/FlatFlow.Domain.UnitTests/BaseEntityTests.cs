using FlatFlow.Domain.Entities;
using FlatFlow.Domain.Enums;
using FlatFlow.Domain.ValueObjects;
using FluentAssertions;

namespace FlatFlow.Domain.UnitTests
{
    public class BaseEntityTests
    {
        private readonly Address _validAddress = new("Main St 1", "Warsaw", "00-001", "Poland");

        [Fact]
        public void Equals_SameReference_ReturnsTrue()
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);

            // Act & Assert
            flat.Equals(flat).Should().BeTrue();
        }

        [Fact]
        public void Equals_SameId_ReturnsTrue()
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);
            var chore = flat.AddChore("Title", "Desc", ChoreFrequency.Once);

            // Act
            var found = flat.Chores.First(c => c.Id == chore.Id);

            // Assert
            chore.Equals(found).Should().BeTrue();
        }

        [Fact]
        public void Equals_DifferentId_ReturnsFalse()
        {
            // Arrange
            var flat1 = new Flat("Flat 1", _validAddress);
            var flat2 = new Flat("Flat 2", _validAddress);

            // Act & Assert
            flat1.Equals(flat2).Should().BeFalse();
        }

        [Fact]
        public void Equals_DifferentType_ReturnsFalse()
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);
            var chore = new Chore("Title", "Desc", ChoreFrequency.Once, Guid.NewGuid());

            // Act & Assert
            flat.Equals(chore).Should().BeFalse();
        }

        [Fact]
        public void Equals_Null_ReturnsFalse()
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);

            // Act & Assert
            flat.Equals(null).Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_SameId_ReturnsSameHash()
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);
            var chore = flat.AddChore("Title", "Desc", ChoreFrequency.Once);
            var found = flat.Chores.First(c => c.Id == chore.Id);

            // Act & Assert
            chore.GetHashCode().Should().Be(found.GetHashCode());
        }

        [Fact]
        public void OperatorEquals_TwoEntitiesWithSameReference_ReturnsTrue()
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);
            var same = flat;

            // Act & Assert
            (flat == same).Should().BeTrue();
        }

        [Fact]
        public void OperatorEquals_BothNull_ReturnsTrue()
        {
            // Arrange
            Flat? left = null;
            Flat? right = null;

            // Act & Assert
            (left == right).Should().BeTrue();
        }

        [Fact]
        public void OperatorEquals_OneNull_ReturnsFalse()
        {
            // Arrange
            var flat = new Flat("My Flat", _validAddress);
            Flat? nullFlat = null;

            // Act & Assert
            (flat == nullFlat).Should().BeFalse();
            (nullFlat == flat).Should().BeFalse();
        }

        [Fact]
        public void OperatorNotEquals_DifferentIds_ReturnsTrue()
        {
            // Arrange
            var flat1 = new Flat("Flat 1", _validAddress);
            var flat2 = new Flat("Flat 2", _validAddress);

            // Act & Assert
            (flat1 != flat2).Should().BeTrue();
        }
    }
}
