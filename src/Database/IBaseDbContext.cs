using BenjaminAbt.EntityFrameworkDemo.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BenjaminAbt.EntityFrameworkDemo.Database;

/// <summary>
/// This interface is the base for all DbContext interfaces. It provides methods for querying and saving instances of TEntity, 
/// marking entities for deletion, attaching and detaching entities, and saving changes to the database.
/// </summary>
public interface IBaseDbContext : IDisposable
{
    /// <summary>
    /// Creates a DbSet that can be used to query and save instances of TEntity.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity for which a set should be returned.</typeparam>
    /// <returns>A set for the given entity type.</returns>
    DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity;

    /// <summary>
    /// Marks the given entity for deletion.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity that should be deleted.</typeparam>
    /// <param name="entity">The entity to delete.</param>
    /// <returns>The EntityEntry for the entity that was marked for deletion.</returns>
    EntityEntry<TEntity> Delete<TEntity>(TEntity entity) where TEntity : BaseEntity;

    /// <summary>
    /// Attaches the given entity to the context underlying the DbSet. That is, the entity is placed into the context in the Unchanged state, 
    /// just as if it had been read from the database.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entry">The entity to attach.</param>
    /// <returns>The EntityEntry for the attached entity.</returns>
    EntityEntry<TEntity> Attach<TEntity>(TEntity entry) where TEntity : BaseEntity;

    /// <summary>
    /// Begins tracking the given entity, and any other reachable entities that are not already being tracked, in the Added state such that 
    /// they will be inserted into the database when SaveChanges is called.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entry">The entity to insert.</param>
    /// <returns>The EntityEntry for the inserted entity.</returns>
    EntityEntry<TEntity> Entry<TEntity>(TEntity entry) where TEntity : BaseEntity;

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <param name="ct">A CancellationToken instance that propagates notification that operations should be canceled.</param>
    /// <returns>The number of state entries written to the database.</returns>
    ValueTask<DbSaveChangesResult> SaveChangesAsync(CancellationToken ct)
        => SaveChangesAsync(updateChangeTrackerStates: true, ct);

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <param name="updateChangeTrackerStates">A boolean flag indicating whether the change tracker states should be updated.</param>
    /// <param name="ct">A CancellationToken instance that propagates notification that operations should be canceled.</param>
    /// <returns>The number of state entries written to the database.</returns>
    ValueTask<DbSaveChangesResult> SaveChangesAsync(bool updateChangeTrackerStates, CancellationToken ct);

    /// <summary>
    /// Adds conventions to the model builder.
    /// </summary>
    /// <param name="modelBuilder">The model builder to which the conventions should be added.</param>
    void AddConventions(ModelBuilder modelBuilder);
}
