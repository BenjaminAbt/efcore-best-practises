using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BenjaminAbt.EntityFrameworkDemo.Database.Converters;

///<summary>
/// A static class that provides an extension method to use value converter for properties that are strongly-typed ID in the EF Core model.
///</summary>
public static class ModelBuilderExtensions
{
    ///<summary>
    /// Extension method, use value converter for strongly-typed ID value in the EF Core model.
    ///</summary>
    ///<param name = "modelBuilder">The ModelBuilder object.</param>
    ///<param name = "converter">value converter.</param>
    ///<returns>This returns a reference to the ModelBuilder after the operation has completed.</returns>
    public static ModelBuilder UseValueConverter(this ModelBuilder modelBuilder, ValueConverter converter)
    {
        Type type = converter.ModelClrType;

        foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
        {
            // find the properties that are our strongly-typed ID
            IEnumerable<PropertyInfo> properties = entityType
                .ClrType
                .GetProperties()
                .Where(p => p.PropertyType == type);

            foreach (PropertyInfo property in properties)
            {
                modelBuilder
                    .Entity(entityType.Name)
                    .Property(property.Name)
                    .HasConversion(converter);
            }
        }

        return modelBuilder;
    }
}
