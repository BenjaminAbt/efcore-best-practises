namespace BenjaminAbt.EntityFrameworkDemo.Database.SqlServer;

/// <summary>
/// Base configuration class for SQL Server database context, provides database connection information.
/// </summary>
public abstract class SqlServerDbContextConfig
{
    /// <summary>
    /// Gets or sets the server hostname, which cannot be null.
    /// </summary>
    public string ServerHostname { get; set; } = null!;

    /// <summary>
    /// Get or sets the database name, which cannot be null.
    /// </summary>
    public string DatabaseName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the connection string template, which cannot be null.
    /// </summary>
    public string ConnectionStringTemplate { get; set; } = null!;

    /// <summary>
    /// Gets or sets a value indicating whether logging is enabled, which defaults to false.
    /// </summary>
    public bool LoggingEnabled { get; set; } = false;
}
