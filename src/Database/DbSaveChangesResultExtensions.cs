namespace BenjaminAbt.EntityFrameworkDemo.Database;

/// <summary>
/// Extension methods for <see cref="DbSaveChangesResult"/>.
/// </summary>
public static class DbSaveChangesResultExtensions
{
    /// <summary>
    /// Throws a <see cref="DatabaseOperationFailedException"/> if the <see cref="DbSaveChangesResult"/> has no results.
    /// </summary>
    /// <param name="context">The <see cref="ValueTask{DbSaveChangesResult}"/> to check.</param>
    /// <returns>The <see cref="DbSaveChangesResult"/> if it has results.</returns>
    public static async ValueTask<DbSaveChangesResult> ThrowIfNoResults(this ValueTask<DbSaveChangesResult> context)
    {
        DbSaveChangesResult result = await context.ConfigureAwait(false);
        if (result.ResultCount is 0) throw new DatabaseOperationFailedException();

        return result;
    }

    /// <summary>
    /// Throws a <see cref="DatabaseOperationFailedException"/> if the <see cref="DbSaveChangesResult"/> does not have the expected number of results.
    /// </summary>
    /// <param name="context">The <see cref="ValueTask{DbSaveChangesResult}"/> to check.</param>
    /// <param name="expected">The expected number of results.</param>
    /// <returns>The <see cref="DbSaveChangesResult"/> if it has the expected number of results.</returns>
    public static async ValueTask<DbSaveChangesResult> ThrowIfNotResult(this ValueTask<DbSaveChangesResult> context, int expected)
    {
        DbSaveChangesResult result = await context.ConfigureAwait(false);
        if (result.ResultCount != expected) throw new DatabaseOperationFailedException();

        return result;
    }

    /// <summary>
    /// Returns a boolean indicating whether the <see cref="DbSaveChangesResult"/> has any rows.
    /// </summary>
    /// <param name="context">The <see cref="ValueTask{DbSaveChangesResult}"/> to check.</param>
    /// <returns>True if the <see cref="DbSaveChangesResult"/> has any rows, otherwise false.</returns>
    public static async ValueTask<bool> HasRows(this ValueTask<DbSaveChangesResult> context)
    {
        DbSaveChangesResult result = await context.ConfigureAwait(false);
        return (result.ResultCount > 0);
    }

    /// <summary>
    /// Returns the number of rows updated by the <see cref="DbSaveChangesResult"/>.
    /// </summary>
    /// <param name="context">The <see cref="ValueTask{DbSaveChangesResult}"/> to check.</param>
    /// <returns>The number of rows updated by the <see cref="DbSaveChangesResult"/>.</returns>
    public static async ValueTask<int> GetUpdatedRowCount(this ValueTask<DbSaveChangesResult> context)
    {
        DbSaveChangesResult result = await context.ConfigureAwait(false);
        return result.ResultCount;
    }
}
