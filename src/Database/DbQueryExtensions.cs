using Microsoft.EntityFrameworkCore;

namespace BenjaminAbt.EntityFrameworkDemo.Database;

/// <summary>
/// Provides Db Query helper extension methods for <see cref="IQueryable{T}"/> instances.
/// </summary>
public static class DbQueryExtensions
{
    /// <summary>
    /// Disables the Entity Tracking if <see cref="DbTrackingOptions.Disabled"/> is provided.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="set">A <see cref="IQueryable{T}"/> to apply tracking options to.</param>
    /// <param name="trackingOptions">An implementation of the <see cref="DbTrackingOptions"/> class containing tracking option information.</param>
    /// <returns>A <see cref="IQueryable{T}"/> with the specified <paramref name="trackingOptions"/> applied.</returns>
    public static IQueryable<T> With<T>(this IQueryable<T> set, DbTrackingOptions trackingOptions) where T : class
    {
        if (trackingOptions is DbTrackingOptions.Disabled)
        {
            set = set.AsNoTracking();
        }

        return set;
    }
}
