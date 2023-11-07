using BenjaminAbt.EntityFrameworkDemo.Database.SqlServer.Logging;
using EntityFramework.Exceptions.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BenjaminAbt.EntityFrameworkDemo.Database.SqlServer;

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
    public static void AddPlatformSqlServerContext<TInterface, TDbContext>(
        this IServiceCollection services, SqlServerDbContextConfig config)
        where TInterface : class, IBaseDbContext
        where TDbContext : DbContext, TInterface
    {
        services.AddDbContextPool<TDbContext>((provider, o) =>
        {
            ILogger logger = provider.GetRequiredService<ILogger<TDbContext>>();

            string template = config.ConnectionStringTemplate;
            string connectionString = string.Format(template, config.ServerHostname, config.DatabaseName);

            o.UseSqlServer(connectionString); // no migration parameter here!

            // log
            Type dbContextType = typeof(TDbContext);
            string dbContextTypeName = dbContextType.FullName ?? "Unknown DbContext Type";

            if (config.LoggingEnabled)
            {
                SqlServerLogging.LogConnectionString(logger, dbContextTypeName, connectionString);

                o.UseLoggerFactory(provider.GetRequiredService<ILoggerFactory>());
                SqlServerLogging.LoggerFactoryRegistered(logger, dbContextTypeName);
            }
            else
            {
                SqlServerLogging.LoggerFactoryNotRegistered(logger, dbContextTypeName);
            }

            // enable better expressions
            o.UseExceptionProcessor();
        });

        // add pool service resolving
        services.AddScoped<TInterface>(p => p.GetRequiredService<TDbContext>());
    }
}
