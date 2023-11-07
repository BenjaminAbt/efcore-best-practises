namespace BenjaminAbt.EntityFrameworkDemo.Database.Sqlite;

/// <summary>
/// Base configuration class for SQL Server database context, provides database connection information.
/// </summary>
public abstract class SqliteDbContextConfig
{
    /// <summary>
    /// Gets or sets the connection string template, which cannot be null.
    /// </summary>
    public string ConnectionString { get; set; } = null!;
}
