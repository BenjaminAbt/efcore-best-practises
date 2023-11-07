/* 
 * Welcome to the EF Core Demo Sample.
 * 
 * This demo is intended for general and generic use of EF Core and 
 *  covers use with SQL Server and SQLite - for testing.
 * 
 * The goal is to provide the basics for building and using EF Core.
 * 
 * This demo was created in the context of activities for Medialesson GmbH..
 */

using BenjaminAbt.EntityFrameworkDemo.Database;
using BenjaminAbt.EntityFrameworkDemo.Database.Entities;
using BenjaminAbt.EntityFrameworkDemo.Database.Projections;
using BenjaminAbt.EntityFrameworkDemo.Database.Projections.Profiles;
using BenjaminAbt.EntityFrameworkDemo.Database.Repositories;
using BenjaminAbt.EntityFrameworkDemo.Database.Sqlite;
using BenjaminAbt.EntityFrameworkDemo.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

/* 
 * This Program.cs contains the example usage of SQL Lite with EFCore.
 * The structure fits to what would be implemented in unit tests.
 * 
 * Scenario: We want to test a projection.
 * 
 * A projection is a reading operation, so we need to build a
 *   seed within our arrange.
 */


Console.WriteLine("Start");
CancellationToken ct = CancellationToken.None;

// An ID is created on the client, which we use both to add the seed entity and to read the projection.
MyEntityId id = MyEntityId.New();

// We create a SqliteConnection, we share in two contexts.
// This is how Sqlite InMemory works
SqliteConnection connection = SqliteDbContextFactory.CreateSqliteInMemoryConnection();

// We create the context for the seed with the open connection.
using (DemoSqliteDbContext dbContext = SqliteDbContextFactory.CreateContext<DemoSqliteDbContext>(connection))
{
    // We use our EFCore based MyEntityRepository to add a sample entity.
    MyEntityRepository repository = new(dbContext);

    MyEntity newEntity = new(id, "batman");

    repository.Add(newEntity);

    await repository.SaveChangesAsync(ct);
}

// We create the context for the act with the open connection.
using (DemoSqliteDbContext dbContext = SqliteDbContextFactory.CreateContext<DemoSqliteDbContext>(connection))
{
    // We use our EFCore based MyEntityRepository to query.
    MyEntityRepository repository = new(dbContext);

    MyEntityNameProjection? projection = await repository
        // To reduce peformance and overhead, we disable
        //  entity tracking, since it has no effect on projections anyway.
        // Projections are read-only.
        .QueryById(id, DbTrackingOptions.Disabled)
        // We pass along our projection profile, which
        //   provides our projection profile into the Expression Visitor, so that
        //   only relevant database fields are loaded.
        .Select(e => MyEntityNameProjectionProfiles.FromMyEntity(e))
        // we only have one
        .SingleOrDefaultAsync(ct);
}

Console.WriteLine("Done.");
