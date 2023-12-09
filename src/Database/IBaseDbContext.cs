using BenjaminAbt.EntityFrameworkDemo.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BenjaminAbt.EntityFrameworkDemo.Database;

// All of your db context interfaces should use IEFCoreDbContext.
// So we can control which EF Core stuff is available in our repositories.

// You can extend this class e.g. to use transactions...

/// <summary>
/// Base interface for the main DbContext, inherited by others.
/// </summary>
public interface IBaseDbContext : IDisposable
{
    /// <summary>
    /// Creates a DbSet<TEntity> that can be used to query and save instances of TEntity.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity for which a set should be returned.</typeparam>
    /// <returns>A set for the given entity type.</returns>
    DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity;

    /// <summary>
    /// Marks a given TEntity for deletion.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity that should be deleted.</typeparam>
    /// <param name="entity">Entity to delete.</param>
    EntityEntry<TEntity> Delete<TEntity>(TEntity entity) where TEntity : BaseEntity;

    /// <summary>
    /// Attaches the provided entry to the context.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity.</typeparam>
    /// <param name="entry">Entry to attach.</param>
    /// <returns>The newly attached entity.</returns>
    EntityEntry<TEntity> Attach<TEntity>(TEntity entry) where TEntity : BaseEntity;

    /// <summary>
    /// The given entry is not tracked, thus making it a candidate for prospective insert operations.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity.</typeparam>
    /// <param name="entry">Entry to insert.</param>
    /// <returns>The newly inserted entity.</returns>
    EntityEntry<TEntity> Entry<TEntity>(TEntity entry) where TEntity : BaseEntity;

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <param name="ct">CancellationToken instance.</param>
    /// <returns>Number of affected rows.</returns>
    ValueTask<DbSaveChangesResult> SaveChangesAsync(CancellationToken ct)
        => SaveChangesAsync(updateChangeTrackerStates: true, ct);

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <param name="updateChangeTrackerStates">Bool flag.</param>
    /// <param name="ct">CancellationToken instance.</param>
    /// <returns>Number of affected rows.</returns>
    ValueTask<DbSaveChangesResult> SaveChangesAsync(bool updateChangeTrackerStates, CancellationToken ct);
}
