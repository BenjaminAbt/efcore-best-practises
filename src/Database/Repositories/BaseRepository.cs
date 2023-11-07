using BenjaminAbt.EntityFrameworkDemo.Database.Entities;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BenjaminAbt.EntityFrameworkDemo.Database.Repositories;

// This is the basis of my generic, DB server neutral EF Core repository - with a few special features

public abstract class BaseRepository<T> where T : BaseEntity
{
    protected IBaseDbContext DbContext { get; }

    private readonly DbSet<T> _set;
    private readonly IQueryable<T> _expandableSet;


    // Unlike other examples of generic repositories, at this point I want not only the context, but also the DbSet.
    // This lowers the overhead and helps me access the set
    //   more easily to take advantage of special features like Expandable.

    protected BaseRepository(IBaseDbContext dbContext, DbSet<T> entitySet)
    {
        DbContext = dbContext;

        _set = entitySet;

        // AsExpandable is a feature of LinqKit that allows us to define our
        //   own Expression Profiles to create projections in a central location; similar
        //   to what we know from AutoMapper Projections - only better.

        // Why better, see Profile Sample.
        _expandableSet = entitySet.AsExpandable();
    }

    // I override all standard crud methods to allow generic access

    public EntityEntry<T> Add(T entity)
        => _set.Add(entity);

    public void AddRange(IEnumerable<T> entities)
        => _set.AddRange(entities);

    public EntityEntry<T> Attach(T entity)
        => _set.Attach(entity);

    public EntityEntry<T> Delete(T entity)
        => _set.Remove(entity);

    public EntityEntry Remove(T entity)
        => _set.Remove(entity);

    public List<EntityEntry<T>> Delete(IEnumerable<T> entities)
        => entities.Select(Delete).ToList();

    public IQueryable<T> Query(DbTrackingOptions to)
        // In contrast to all other methods, queries
        //   should only be made via the expandable set!
        => _expandableSet.With(to);

    // The SaveChanges methods are also provided, but the return is
    //   a typed struct rather than a non-saying Int

    public Task<DbSaveChangesResult> SaveChangesAsync(CancellationToken ct)
        => DbContext.SaveChangesAsync(ct);

    // Optionally, you can disable the Tracker State Update mechanism if you do not need the overhead.
    public Task<DbSaveChangesResult> SaveChangesAsync(bool updateChangeTrackerStates, CancellationToken ct)
        => DbContext.SaveChangesAsync(updateChangeTrackerStates, ct);
}
