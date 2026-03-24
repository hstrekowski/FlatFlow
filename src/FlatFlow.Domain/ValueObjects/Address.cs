namespace FlatFlow.Domain.ValueObjects
{
    public record Address
    {
        public string Street { get; init; }
        public string City { get; init; }
        public string ZipCode { get; init; }
        public string Country { get; init; }

        public Address(string street, string city, string zipCode, string country)
        {
            if (string.IsNullOrWhiteSpace(street))
                throw new ArgumentException("Street cannot be empty.", nameof(street));
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City cannot be empty.", nameof(city));
            if (string.IsNullOrWhiteSpace(zipCode))
                throw new ArgumentException("Zip code cannot be empty.", nameof(zipCode));
            if (string.IsNullOrWhiteSpace(country))
                throw new ArgumentException("Country cannot be empty.", nameof(country));

            Street = street;
            City = city;
            ZipCode = zipCode;
            Country = country;
        }
    }
}
