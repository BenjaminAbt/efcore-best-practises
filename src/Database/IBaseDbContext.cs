using BenjaminAbt.EntityFrameworkDemo.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BenjaminAbt.EntityFrameworkDemo.Database;

// All of your db context interfaces should use IEFCoreDbContext.
// So we can control which EF Core stuff is available in our repositories.

// You can extend this class e.g. to use transactions...

public interface IBaseDbContext : IDisposable
{
    DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity;

    EntityEntry<TEntity> Delete<TEntity>(TEntity entity) where TEntity : BaseEntity;
    EntityEntry<TEntity> Attach<TEntity>(TEntity entry) where TEntity : BaseEntity;
    EntityEntry<TEntity> Entry<TEntity>(TEntity entry) where TEntity : BaseEntity;

    Task<DbSaveChangesResult> SaveChangesAsync(CancellationToken ct)
        => SaveChangesAsync(updateChangeTrackerStates: true, ct);

    Task<DbSaveChangesResult> SaveChangesAsync(bool updateChangeTrackerStates, CancellationToken ct);
}
