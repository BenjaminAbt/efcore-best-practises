using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BenjaminAbt.EntityFrameworkDemo.Database.Converters;

/// <summary>
/// A converter that converts between <see cref="DateTimeOffset"/> and <see cref="long"/>.
/// </summary>
public class DateTimeOffsetToUtcDateTimeTicksConverter : ValueConverter<DateTimeOffset, long>
{
    /// <summary>
    /// Creates a new instance of <see cref="DateTimeOffsetToUtcDateTimeTicksConverter"/>.
    /// </summary>
    /// <param name="mappingHints">An optional set of hints used by the EF Core runtime.</param>
    public DateTimeOffsetToUtcDateTimeTicksConverter(ConverterMappingHints? mappingHints = null)
        : base(dateTimeOffset => dateTimeOffset.UtcDateTime.Ticks,
            longValue => new DateTimeOffset(longValue, TimeSpan.Zero), mappingHints)
    { }
}
