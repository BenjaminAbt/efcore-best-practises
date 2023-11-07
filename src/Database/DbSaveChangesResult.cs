namespace BenjaminAbt.EntityFrameworkDemo.Database;

// DbSaveChangesResult is an abstraction that helps me to represent the
// standard integer return in a separate struct and to be able to write extension methods on the struct.

/// <summary>
/// Represents the result of a database save operation.
/// </summary>
public readonly record struct DbSaveChangesResult
{
    /// <summary>
    /// Gets the number of entities affected by the save operation.
    /// </summary>
    public int ResultCount { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DbSaveChangesResult"/> struct.
    /// </summary>
    /// <param name="resultCount">The number of entities affected by the save operation.</param>
    public DbSaveChangesResult(int resultCount)
    {
        ResultCount = resultCount;
    }

    /// <summary>
    /// Implicitly converts a <see cref="DbSaveChangesResult"/> object to an <see cref="int"/>.
    /// </summary>
    /// <param name="result">The <see cref="DbSaveChangesResult"/> object to convert.</param>
    /// <returns>The number of entities affected by the save operation.</returns>
    public static implicit operator int(DbSaveChangesResult result)
        => result.ResultCount;

    /// <summary>
    /// Implicitly converts an <see cref="int"/> to a <see cref="DbSaveChangesResult"/> object.
    /// </summary>
    /// <param name="resultCount">The number of entities affected by the save operation.</param>
    /// <returns>A new instance of the <see cref="DbSaveChangesResult"/> struct.</returns>
    public static implicit operator DbSaveChangesResult(int resultCount)
        => new(resultCount);
}
