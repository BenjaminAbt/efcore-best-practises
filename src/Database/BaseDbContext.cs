using BenjaminAbt.EntityFrameworkDemo.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BenjaminAbt.EntityFrameworkDemo.Database;

// This is our custom EFCoreDbContext abstraction.
// It is used to overwrite all underlaying entity type-unsafe CRUD methods

public abstract class BaseDbContext : DbContext, IBaseDbContext
{
    protected BaseDbContext(DbContextOptions options)
        : base(options) { } // Required for dependency injection

    public new DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        => base.Set<TEntity>();

    public EntityEntry<TEntity> Delete<TEntity>(TEntity entity) where TEntity : BaseEntity
        => base.Remove(entity);

    public new EntityEntry<TEntity> Attach<TEntity>(TEntity entry) where TEntity : BaseEntity
        => base.Attach(entry);

    public new EntityEntry<TEntity> Entry<TEntity>(TEntity entry) where TEntity : BaseEntity
        => base.Entry(entry);

    // also we overwrite SaveChangesAsync so we get our custom return type

    public new async Task<DbSaveChangesResult> SaveChangesAsync(bool updateChangeTrackerStates, CancellationToken ct)
         => new(await base.SaveChangesAsync(acceptAllChangesOnSuccess: updateChangeTrackerStates, ct)
                .ConfigureAwait(false));
}
