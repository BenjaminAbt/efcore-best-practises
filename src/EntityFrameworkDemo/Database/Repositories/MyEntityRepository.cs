using BenjaminAbt.EntityFrameworkDemo.Database.Entities;
using BenjaminAbt.EntityFrameworkDemo.Database.Queries;
using BenjaminAbt.EntityFrameworkDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace BenjaminAbt.EntityFrameworkDemo.Database.Repositories;

// The MyEntityRepository is a generic, provider-neutral repository, which works in this form with
//    the Sql Server as well as with the Sqlite Server. Therefore it inherits from BaseRepository.

// If specific server functionalities are to be used, the repository should be
//    MyEntitySqlServerRepository and then also inherit from BaseSqlServerRepository.

// If an application should support multiple database providers, either this neutral repository must be
//    or there must be a specific implementation for each database provider.

public class MyEntityRepository : BaseRepository<MyEntity>
{
    public MyEntityRepository(IMyDemoDbContext dbContext)
        : base(dbContext, dbContext.MyEntities) { }

    // Query
    public IQueryable<MyEntity> QueryById(MyEntityId id, DbTrackingOptions to)
        => Query(to).Where(MyEntityQuery.WithId(id)); // query by Id using our static Expressions
    // because the underlaying Query is using AsExpandable, we dont have to put that onto every query method

    // Get
    public Task<MyEntity?> GetById(MyEntityId id, DbTrackingOptions to)
        => QueryById(id, to).SingleOrDefaultAsync(); // use our QueryById to load an entity

    public Task<int> CountAsync(CancellationToken ct)
        => Query(DbTrackingOptions.Disabled).CountAsync(ct); // count the entities
}
