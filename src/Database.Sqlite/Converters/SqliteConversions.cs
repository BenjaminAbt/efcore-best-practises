using BenjaminAbt.EntityFrameworkDemo.Database.Converters;
using Microsoft.EntityFrameworkCore;

namespace BenjaminAbt.EntityFrameworkDemo.Database.Sqlite.Converters;

public static class SqliteConversions
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        // we have to add a custom value converter because out of the box Sqlite 
        //   does not (wtf!) support DateTimeOffset
        modelBuilder.UseValueConverter(new DateTimeOffsetToUtcDateTimeTicksConverter());
    }
}
