using EntityFramework.Exceptions.Sqlite;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BenjaminAbt.EntityFrameworkDemo.Database.Sqlite;

public static class SqliteDbContextFactory
{
    /// <summary>
    /// Method to create a Sqlite connection for testing purposes
    /// </summary>
    /// <returns>SqliteConnection object for testing</returns>
    public static SqliteConnection CreateSqliteInMemoryConnection()
    {
        SqliteConnection connection = new("DataSource=:memory:");
        connection.Open();
        return connection;
    }

    /// <summary>
    /// Method to create options for the provided SqlServerBaseDbContext
    /// </summary>
    /// <typeparam name="T">Type of SqlServerBaseDbContext</typeparam>
    /// <param name="sqliteConnection">SqliteConnection to provide options for</param>
    /// <returns>DbContextOptions object with options for the provided SqliteConnection</returns>
    public static DbContextOptions<T> CreateOptions<T>(SqliteConnection sqliteConnection) where T : BaseDbContext
        => new DbContextOptionsBuilder<T>()
                   .UseSqlite(sqliteConnection)
                   .EnableSensitiveDataLogging(true)
                   .UseExceptionProcessor()  // enable better expressions
                   .Options;

    /// <summary>
    /// Method to create a DbContext for the provided SqlServerBaseDbContext and connection
    /// </summary>
    /// <typeparam name="T">Type of SqlServerBaseDbContext</typeparam>
    /// <param name="sqliteConnection">SqliteConnection to provide context for</param>
    /// <returns>New instance of the provided SqlServerBaseDbContext connected to the provided SqliteConnection</returns>
    public static T CreateContext<T>(SqliteConnection sqliteConnection) where T : BaseDbContext
    {
        DbContextOptions<T> dbContextOptions = CreateOptions<T>(sqliteConnection);

        T context = (T)Activator.CreateInstance(typeof(T), dbContextOptions)!;
        context.Database.EnsureCreated();

        return context;
    }
}
