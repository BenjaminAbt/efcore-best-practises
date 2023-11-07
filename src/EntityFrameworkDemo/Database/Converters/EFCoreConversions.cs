using Microsoft.EntityFrameworkCore;

namespace BenjaminAbt.EntityFrameworkDemo.Database.Converters;

// Conversions can be used to tell EF Core how to handle custom types.
//   We use custom types for stricter typing to avoid mistakes and typos.

public static class EFCoreConversions
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        // In this case we have a central registration of a custom type, since
        //   this type must always be registered no matter which database provider is used.

        // There are also cases where special types only need to be registered for a
        //   specific DB provider, see the Sqlite example.
        modelBuilder.UseValueConverter(new MyEntityIdValueConverter());
    }
}
