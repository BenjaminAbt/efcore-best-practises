using BenjaminAbt.EntityFrameworkDemo.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BenjaminAbt.EntityFrameworkDemo.Database.Converters;

// With the help of this converter, EF Core knows how to use our own type under the hood.
public class MyEntityIdValueConverter : ValueConverter<MyEntityId, Guid>
{
    public MyEntityIdValueConverter(ConverterMappingHints? mappingHints = null)
        : base(id => id.Value, value => new(value), mappingHints) { }
}
