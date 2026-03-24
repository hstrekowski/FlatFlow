using System;
using System.Collections.Generic;
using System.Text;

namespace FlatFlow.Domain.ValueObjects
{
    public record Address(string Street, string City, string ZipCode, string Country);
}
