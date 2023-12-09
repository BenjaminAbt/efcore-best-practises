using BenjaminAbt.EntityFrameworkDemo.Database.Entities;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BenjaminAbt.EntityFrameworkDemo.Database.Repositories;

// This is the basis of my generic, DB server neutral EF Core repository - with a few special features


public abstract class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
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
    ///<summary>
    ///The method adds an entity to the database.
    ///</summary>
    ///<param name="entity"> The entity to be added to the database. </param>
    ///<returns> EntityEntry instance corresponding to the entity. </returns>
    public EntityEntry<T> Add(T entity)
        => _set.Add(entity);

    ///<summary>
    ///Adds a range of entities to the database.
    ///</summary>
    ///<param name="entities"> The entities to be added to the database </param>
    public void AddRange(IEnumerable<T> entities)
        => _set.AddRange(entities);

    ///<summary>
    ///Attaches the entity to the database.
    ///</summary>
    ///<param name="entity"> The entity to be attached. </param>
    ///<returns> EntityEntry instance corresponding to the entity. </returns>
    public EntityEntry<T> Attach(T entity)
        => _set.Attach(entity);

    ///<summary>
    ///Deletes entity from the database.
    ///</summary>
    ///<param name="entity"> The entity to be deleted. </param>
    ///<returns> EntityEntry instance corresponding to the entity. </returns>
    public EntityEntry<T> Delete(T entity)
        => _set.Remove(entity);

    /// <summary>
    /// Deletes a range of entities from the database.
    /// </summary>
    /// <param name="entities">The entities to delete.</param>
    /// <returns>A list of deleted entities.</returns>
    public List<EntityEntry<T>> Delete(IEnumerable<T> entities)
        => entities.Select(Delete).ToList();

    ///<summary>
    ///The method returns an IQueryable of T for a given database tracking option.
    ///</summary>
    ///<param name="to"> The database tracking option. </param>
    ///<returns> IQueryable instance of T type. </returns>
    public IQueryable<T> Query(DbTrackingOptions to)
        => _expandableSet.With(to);

    // The SaveChanges methods are also provided, but the return is
    //   a typed struct rather than a non-saying Int

    ///<summary>
    ///Saves changes to the database using a cancellation token.
    ///</summary>
    ///<param name="ct"> Cancellation token. </param>
    ///<returns> An instance of DbSaveChangesResult. </returns>
    public ValueTask<DbSaveChangesResult> SaveChangesAsync(CancellationToken ct)
        => DbContext.SaveChangesAsync(ct);

    ///<summary>
    ///Saves changes to the database with an option to update change state.
    ///</summary>
    ///<param name="updateChangeTrackerStates"> Boolean indicator to update change tracker states. </param>
    ///<param name="ct"> Cancellation token. </param>
    ///<returns> An instance of DbSaveChangesResult. </returns>
    public ValueTask<DbSaveChangesResult> SaveChangesAsync(bool updateChangeTrackerStates, CancellationToken ct)
        => DbContext.SaveChangesAsync(updateChangeTrackerStates, ct);
}
