using EntityFramework.Exceptions.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BenjaminAbt.EntityFrameworkDemo.Database.Sqlite;

/// <summary>
/// Extension methods for adding the SQLServer DbContext to the DI container.
/// </summary>
public static class DbContextExtensions
{
    /// <summary>
    /// Adds the SQLServer DbContext to the DI container for the specified interface and DbContext types.
    /// </summary>
    /// <typeparam name="TInterface">The interface type implemented by the DbContext.</typeparam>
    /// <typeparam name="TDbContext">The DbContext type.</typeparam>
    /// <param name="services">The collection of service descriptors.</param>
    /// <param name="config">The SQLServer DbContext configuration object.</param>
    public static void AddPlatformSqliteContext<TInterface, TDbContext>(
        this IServiceCollection services, SqliteDbContextConfig config)
        where TInterface : class, IBaseDbContext
        where TDbContext : DbContext, TInterface
    {
        services.AddDbContextPool<TDbContext>((provider, o) =>
        {
            string connectionString = config.ConnectionString;

            o.UseSqlite(connectionString);

            // enable better expressions
            o.UseExceptionProcessor();
        });

        // add pool service resolving
        services.AddScoped<TInterface>(p => p.GetRequiredService<TDbContext>());
    }
}
