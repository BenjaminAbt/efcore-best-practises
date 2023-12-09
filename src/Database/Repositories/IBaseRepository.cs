using BenjaminAbt.EntityFrameworkDemo.Database.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BenjaminAbt.EntityFrameworkDemo.Database.Repositories;

/// <summary>
/// Base Repository Interface for all entities.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IBaseRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Adds the specified entity to the database.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>The added entity.</returns>
    EntityEntry<T> Add(T entity);

    /// <summary>
    /// Adds a range of entities to the database.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    void AddRange(IEnumerable<T> entities);

    /// <summary>
    /// Attaches an entity to the database.
    /// </summary>
    /// <param name="entity">The entity to attach.</param>
    /// <returns>The attached entity.</returns>
    EntityEntry<T> Attach(T entity);

    /// <summary>
    /// Deletes the specified entity from the database.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <returns>The deleted entity.</returns>
    EntityEntry<T> Delete(T entity);

    /// <summary>
    /// Deletes a range of entities from the database.
    /// </summary>
    /// <param name="entities">The entities to delete.</param>
    /// <returns>A list of deleted entities.</returns>
    List<EntityEntry<T>> Delete(IEnumerable<T> entities);

    /// <summary>
    /// Returns a queryable instance of entities with an option to track changes or not.
    /// </summary>
    /// <param name="to">A value indicating whether to track changes or not.</param>
    /// <returns>The queryable instance of entities.</returns>
    IQueryable<T> Query(DbTrackingOptions to);

    /// <summary>
    /// Asynchronously saves all changes made in the database.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    ValueTask<DbSaveChangesResult> SaveChangesAsync(CancellationToken ct);

    /// <summary>
    /// Asynchronously saves all changes made in the database, with an option to update the change tracker state or not.
    /// </summary>
    /// <param name="updateChangeTrackerStates">A value indicating whether to update the change tracker state or not.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    ValueTask<DbSaveChangesResult> SaveChangesAsync(bool updateChangeTrackerStates, CancellationToken ct);
}
