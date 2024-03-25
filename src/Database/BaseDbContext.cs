using BenjaminAbt.EntityFrameworkDemo.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BenjaminAbt.EntityFrameworkDemo.Database;

/// <summary>
/// This is our custom EFCoreDbContext abstraction.
/// It is used to overwrite all underlaying entity type-unsafe CRUD methods
/// </summary>
public abstract class BaseDbContext : DbContext, IBaseDbContext
{
    /// <summary>
    /// Constructor for BaseDbContext class
    /// </summary>
    /// <param name="options">Database context options</param>
    protected BaseDbContext(DbContextOptions options)
        : base(options) { }

    /// <summary>
    /// This method is called when the model for a derived context has been initialized,
    /// but before the model has been locked down and used to initialize the context.
    /// The default implementation of this method does nothing,
    /// but it can be overridden in a derived class such that the model can be further
    /// configured before it is locked down.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        AddConventions(modelBuilder);
    }

    /// <summary>
    /// Get and set DbSet
    /// </summary>
    /// <typeparam name="TEntity">Type of entity</typeparam>
    /// <returns>The entity in the set</returns>
    public new DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        => base.Set<TEntity>();

    /// <summary>
    /// Remove entity from the context
    /// </summary>
    /// <typeparam name="TEntity">Type of entity</typeparam>
    /// <param name="entity">The entity to remove</param>
    /// <returns>The deleted entity object</returns>
    public EntityEntry<TEntity> Delete<TEntity>(TEntity entity) where TEntity : BaseEntity
        => base.Remove(entity);

    /// <summary>
    /// Attach entity into the context
    /// </summary>
    /// <typeparam name="TEntity">Type of entity</typeparam>
    /// <param name="entry">The entry to attach</param>
    /// <returns>The attached entity object</returns>
    public new EntityEntry<TEntity> Attach<TEntity>(TEntity entry) where TEntity : BaseEntity
        => base.Attach(entry);

    /// <summary>
    /// Get entry from context
    /// </summary>
    /// <typeparam name="TEntity">Type of entity</typeparam>
    /// <param name="entry">The entity entry to return</param>
    /// <returns>The entity entry requested</returns>
    public new EntityEntry<TEntity> Entry<TEntity>(TEntity entry) where TEntity : BaseEntity
        => base.Entry(entry);

    /// <summary>
    /// Save changes to the database asynchronously
    /// </summary>
    /// <param name="updateChangeTrackerStates">true to update the change tracker before saving</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>DbSaveChangesResult object containing the number of affected entities and validation errors</returns>
    public new async ValueTask<DbSaveChangesResult> SaveChangesAsync(bool updateChangeTrackerStates, CancellationToken ct)
    {
        int rows = await base
            .SaveChangesAsync(acceptAllChangesOnSuccess: updateChangeTrackerStates, ct)
            .ConfigureAwait(false);

        return new(rows);
    }

    /// <summary>
    /// Add conventions to the model builder. This method is empty and can be overridden in derived classes.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    public void AddConventions(ModelBuilder modelBuilder) { }
}
