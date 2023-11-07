## EntityFramework Multi-Database Demo and personal best-practises by BEN ABT

![image](./docs/head.png)

This example demonstrates the modern use of Entity Framework in highly scalable and flexible SaaS applications, which I run myself as well as have successfully established in multiple customer projects.

I show basics as well as significantly advanced mechanisms that may seem as overhead or complex at first glance, but offer enormous advantages in the respective scenarios. Especially in terms of maintainability, testability (almost all examples are 100% unit testable) and performance.

This includes:

- Generic entities
- Generic and specific entity configurations
- Generic and specific repositories
- Custom-Types (here for Entity-Ids)
- Efficient re-use of Entity Query Expressions
- Selections, projections and profiles through expandables for efficient and performance queries
- The use of multiple database servers (here as an example with SqlServer and Sqlite)
- Unit testing with Sqlite
- DevOps mechanism (here for SqlServer) through EF Migrations and EF Migration Bundles
- Best practices for Entity Framework


## Entities

Entities represent the structure of data storage in e.g. a database. In this example it is assumed that an entity can be used in its structure for several databases. Theoretically, it is possible that different entity types(=classes) and contents can be addressed per database, but this is a rare and rather complex scenario.

The entity in this example has only an id (with a custom type) and a name to keep it simple. 

```csharp
public sealed class MyEntity : BaseEntity
{
    // There are certain EF Core features (such as migrations or testing) 
    //  that always require an empty constructor.
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public MyEntity() { }

    // Default constructor to create our entity
    public MyEntity(MyEntityId id, string name)
    {
        Id = id;
        Name = name;
    }

    // our sample properties

    public MyEntityId Id { get; set; } = null!; // is using our custom Id Type!

    [MaxLength(4000)]
    [Unicode(true)]
    public string Name { get; set; } = null!;
}
```

However, the entity itself inherits from `BaseEntity`, which is just an abstract wrapper; but gives us the possibility to implement generic implementations based on this base.

## Entity Configuration

Entity configurations are information used to primarily represent the schema of a database. They specify which properties are mandatory, which have certain rules like minimum length or range.
The configuration can be done within the entity by attributes from the [System.ComponentModel.DataAnnotations](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations?view=net-7.0&WT.mc_id=DT-MVP-5001507) namespace, or in the configuration files per database implementation.

Advantages of each option:
- Data annotations can be assigned centrally and allow validation of the entity instance.
- Configurations are individual per database implementation, and thus can provide more optimizations. Commonalities can be solved centrally by inheritance accordingly.

Sample of the [SqlServer Config](./src/EntityFrameworkDemo/Database/SqlServer/Configs/MyEntitySqlServerConfig.cs)
```csharp
public class MyEntitySqlServerConfig : IEntityTypeConfiguration<MyEntity>
{
    public void Configure(EntityTypeBuilder<MyEntity> b)
    {
        b.ToTable("MyEntities");

        b.HasKey(e => e.Id);

        // you can add custom Sqlite configs here.
        // e.g. limit name length to 8000 (which is the max indexable length)
        b.Property(e => e.Name)
            .IsRequired()
            .IsUnicode(true)
            .HasMaxLength(8000);
    }
}
```

> Personally, I even prefer the mix of both in this case: validatable things like Range or MaxLength by attributes, relation or database configurations in the config.

## Custom Types

In principle, the Entity Framework can handle almost any type out of the box. However, certain scenarios require custom types and associated converters. This is the case if you want to do mappings, for example.

I use custom types mainly to be able to represent strong-typing of content.

There is nothing worse than when a collection of parameters has the same type, but the meaning is immensely different.
In the case of database ids, this can happen very quickly when you have queries that combine several of them, for example

```csharp
public Task<MyUserEntity?> GetBy(Guid tenantId, Guid userId)
```

Although the interpretation is completely different, both parameters are of the type `Guid`. So there is no strong typing.This is not great with or without database interaction.
Unfortunately, C# does not (yet) have [Type Abbreviations](https://learn.microsoft.com/dotnet/fsharp/language-reference/type-abbreviations?WT.mc_id=DT-MVP-5001507) as known from other languages like TypeScript or F#. Therefore we have to help ourselves out with an construct of custom classes.

See [MyEntityId(Guid)](./src/EntityFrameworkDemo/Models/MyEntityId.cs)
```csharp
// This is our custom id, which has a Guid under the hood and implements the appropriate
//    comparison methods so that EF Core can correctly handle our type.

// On top of that, there are a few other methods that make our lives easier.
// In my productive applications I use for automatic generation appropriate
//    source code generators.

// This example here should show the full implementation.

public sealed record class MyEntityId(Guid Value)
    : IPlatformIdentifierGuid<MyEntityId>
{
    public MyEntityId(string val) : this(Guid.Parse(val)) { }

    public static MyEntityId New() => new(Guid.NewGuid());

    public int CompareTo(MyEntityId? other) => Value.CompareTo(other?.Value);

    public override string ToString() => Value.ToString();

    [return: NotNullIfNotNull(nameof(source))]
    public static List<MyEntityId>? From(IEnumerable<Guid>? source)
        => source?.Select(e => new MyEntityId(e)).ToList();
}
```
The value must remain a Guid on the database; but be a separate type at sound time. The solution: [EF Core Value Converters](https://learn.microsoft.com/ef/core/modeling/value-conversions?WT.mc_id=DT-MVP-5001507).

Converters are registered in the [DB Context](./src/EntityFrameworkDemo/Database/Converters/EFCoreConversions.cs), which allows the Entity Framework to handle the custom types accordingly.

```csharp
// With the help of this converter, EF Core knows how to use our own type under the hood.
public class MyEntityIdValueConverter : ValueConverter<MyEntityId, Guid>
{
    public MyEntityIdValueConverter(ConverterMappingHints? mappingHints = null)
        : base(id => id.Value, value => new(value), mappingHints) { }
}
```

Converters help us not only with own types, but generally also with handling data types, which are not available on certain databases. So this example also includes a [DateTimeOffsetConverter](./src/Database.Sqlite/Converters/SqliteConversions.cs), because Sqlite does not know this data type and EF Core accordingly cannot store or read values here. The `DateTimeOffsetConverter` makes sure that values can be stored as long.

The structure of the converters in this example is in such a way that each context knows its converter (e.g. DateTimeOffsetConverter specifically for Sqlite) and own can be added.

See also my blog post [Better Code with custom Strong-Id Types](https://medium.com/medialesson/better-code-with-custom-strong-id-types-9467b33153fe).

## Database Contexts

The database context is the heart of database handling. What at first glance seems a little over-engineered in this demo has its uses, however - because all other possibilities that you want to implement with EF Core are based on the flexibility and the possibilities that you have created for yourself with the DbContext.

The structure of the context therefore follows the following main scenarios:
- An application in which only one database is permanently supported (e.g. only MSSQL)
- An application that has several domains / several databases (modular monolith) and requires different contexts (one database = one context)
- Optional additional possibility that an application uses different databases for the same domain (e.g. tenant A = Postgres, tenant B = MSSQL) and must therefore be able to create contexts dynamically.

>  The bottom line is that it depends on the application which path is taken. The first approach, where an application only has one database, is of course the simplest. However, in cloud scenarios, where there is a tendency to over-engineer with microservices and to run oneself into the wall, the second option is often the better one.

As a base this example has both generic contexts (in form of an EF DB Context), which work with every database (see [BaseDbContext](./src/Database/BaseDbContext. cs)), which EF Core supports generically or specific database contexts, which only work with MSSQL (see [SqlServerBaseDbContext](./src/Database.SqlServer/SqlServerBaseDbContext.cs)) or only with Sqlite (see [SqliteBaseDbContext](./src/Database.Sqlite/SqliteBaseDbContext.cs)).

>  The simplest scenario is to simply inherit from the `BaseDbContext` and not have specific repositories just for one specific database. However, this also means that all other functionalities - such as the repositories - must also comply with this.

The difference in the contexts is actually only that the specific contexts bring specific configurations, e.g. a DateTimeOffset converter for Sqlite, since this type is not supported natively.

A sample implementation might look like this:

```csharp
public class DemoSqlServerDbContext : SqlServerBaseDbContext, IMyDemoDbContext
{
    public DemoSqlServerDbContext(DbContextOptions<DemoSqlServerDbContext> options)
        : base(options) { }

    public DbSet<MyEntity> MyEntities => Set<MyEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // we have to apply OnModelCreating to get the Sql Server conversions
        base.OnModelCreating(modelBuilder);

        // apply general EF Core conversions
        EFCoreConversions.Apply(modelBuilder);

        // apply sql server config
        modelBuilder.ApplyConfiguration(new MyEntitySqlServerConfig());
    }
}
```

So we inherit from the specific `SqlServerBaseDbContext` and have some additional configurations:

- We inject the configurations of the base class `SqlServerBaseDbContext`.
- We register Custom EF Core Conversions - in our case the Id conversion for our Strong-Typed Ids
- We register the entity configurations

With this, we have completed the configuration of MSSQL and can implement this for other database types as well, if desired.

## Database Contexts - Extensions

The [BaseDbContext](./src/Database/BaseDbContext.cs) class is implemented or prepared in such a way that it can offer certain extensions to make certain conversions easier.

For example, the method `SaveChangesAsync` returns its own type `DbSaveChangesResult` instead of just an integer with the number of affected lines. The reason for this is that it is easier to apply extension methods to custom types than to standard types such as integer.

I use this, for example, to validate database operations. So I want to have an exception if no rows or not the accepted rows have been changed.

```csharp
public static class DbSaveChangesResultExtensions
{
    /// <summary>
    /// Throws a <see cref="DatabaseOperationFailedException"/> if the <see cref="DbSaveChangesResult"/> does not have the expected number of results.
    /// </summary>
    /// <param name="context">The <see cref="ValueTask{DbSaveChangesResult}"/> to check.</param>
    /// <param name="expected">The expected number of results.</param>
    /// <returns>The <see cref="DbSaveChangesResult"/> if it has the expected number of results.</returns>
    public static async ValueTask<DbSaveChangesResult> ThrowIfNotResult(this ValueTask<DbSaveChangesResult> context, int expected)
    {
        DbSaveChangesResult result = await context.ConfigureAwait(false);
        if (result.ResultCount != expected) throw new DatabaseOperationFailedException();

        return result;
    }
}

// will be used as

List<MyEntity> entitiesToAdd = .. // 10 items

_myRepository.Add(entitiesToAdd);

await _myRepository.SaveChangesAsync(ct).ThrowIfNotResult(entitiesToAdd.Count); 
```

See [DbSaveChangesResultExtensions](./src/Database/DbSaveChangesResultExtensions.cs)

### Repositories

The idea of the structure of the repositories is very similar and closely wired to the idea of the context:

- There are general repositories that exclusively use EFCore functionalities and thus support all databases that EF Core also supports
- There are specific repositories per database management system that support this one system (e.g. MSSQL) and can have specific adaptations / optimizations for it (e.g. custom MSSQL SQL code).

It is also possible or designed that repositories can be loaded dynamically so that different databases can be supported in runtime - for example, because tenant A uses Azure SQL for user administration and tenant B uses a Postgres system.

So the simple option is for a repository to take the [BaseRepository](./src/Database/Repositories/BaseRepository.cs) directly as a basis and not need any database-specific implementation, or to take the specific route and inherit from [SqlServerBaseRepository](./src/Database.SqlServer/Repositories/SqlServerBaseRepository.cs), for example.

```csharp
// Simple
public interface IMyEntityRepository 
    : IBaseRepository<MyEntity> { }

public class MyEntityRepository // Uses EF Core defaults only
    : BaseRepository<MyEntity>, IMyEntityRepository { }

// Specific
public interface IMyEntityRepository
    : IBaseRepository<MyEntity> { }

public class MyEntityMssqlRepository // MSSQL specific
    : SqlServerBaseRepository<MyEntity>, IMyEntityRepository { }

public class MyEntityPostgresRepository // Postgres specific
    : PostgresBaseRepository<MyEntity>, IMyEntityRepository { }

public class MyEntitySqliteRepository // Sqlite specific
    : SqliteBaseRepository<MyEntity>, IMyEntityRepository { }
```

The idea of the repository functionality is to provide optimized, common methods.

In addition to the standard query method (IQueryable on all elements, located in and inherited of BaseRepository), specific methods are to be made available for each entity, e.g. the selection of the entity based on the ID.

```csharp
public class MyEntityRepository : BaseRepository<MyEntity>
{
    public MyEntityRepository(IMyDemoDbContext dbContext)
        : base(dbContext, dbContext.MyEntities) { }

    // Query
    public IQueryable<MyEntity> QueryById(MyEntityId id, DbTrackingOptions to)
        => Query(to).Where(MyEntityQuery.WithId(id));
```

Further queries can be individual or based on another query, e.g.

```csharp
    public IQueryable<MyEntity> QueryIsActiveById(MyEntityId id, DbTrackingOptions to)
        => QueryById(to).Where(MyEntityQuery.IsActive());
```

Dies gilt auch bei materialisiertenden Methoden:


```csharp
    // Get
    public Task<MyEntity?> GetById(MyEntityId id, DbTrackingOptions to)
        => QueryById(id, to).SingleOrDefaultAsync(); // use our QueryById to load an entity

    public Task<int> CountAsync(CancellationToken ct)
        => Query(DbTrackingOptions.Disabled).CountAsync(ct); // count the entities
```

Full sample see [MyEntityRepository](./src/EntityFrameworkDemo/Database/Repositories/MyEntityRepository.cs)

*It is highly recommended that logic classes access queries directly to materialize them and that no `SaveChanges` are used in the repository itself.

```csharp
// MyUserSampleHandler.cs
UserEntity? user = await userRepository
        .GetById(myUserId, DbTrackingOptions.Enabled)
        .ConfigureAwait(false);

UserRoleEntity? userRole = await userRoleRepository
        .GetById(myUserId, DbTrackingOptions.Enabled)
        .Include(role => role.Claims)
        .ConfigureAwait(false);

// ....

await userRepository
        .SaveChanges(ct)
        .ThrowIfNoResult()
        .ConfigureAwait(false);
```

Otherwise, it is not possible for data to be read and/or written across multiple repositories within a transaction, which reduces efficiency but can increase the probability of errors.* 

## Repository - Extensions

The repository has certain extensions here, e.g. the [DbTrackingOptions](./src/Database/DbTrackingOptions.cs) parameter.
This parameter tells the context as an nNum whether tracking should be deactivated (e.g. in read-only operations), which gives a performance boost.

```csharp
/// <summary>
/// Specifies whether entity tracking is enabled or disabled.
/// </summary>
public enum DbTrackingOptions
{
    /// <summary>
    /// Tracking is enabled.
    /// </summary>
    Enabled,
    /// <summary>
    /// Tracking is disabled.
    /// </summary>
    Disabled
}
```

I am a proponent of the explicit variant, as it has become clear in everyday life that in many of my (supervised) projects without parameter enforcement, this optimization is not taken into account / too often forgotten.
This is almost impossible to cover with unit tests without immense effort, too.

Specifying the parameter as mandatory has led to significantly better performing and more conscious code, which has made a positive contribution to the quality of the application.

## Entity Query Expressions

Entity Query Expressions are a mechanism to deal more efficiently with data queries over Linq. Often you see source code like

```csharp
List<MyEntity> activeEntities = dbContext.MyEntities
                .Where(entity => entity.ActivatedOn != null)
                .ToListAsync(ct);
```

This is not a problem in principle - the problem comes when you have to change these where-clauses. This leads in most cases to bugs, because you don't update all places, if the filter query has changed logically.

Also at this point it may be noted: dont repeat yourself. You are allowed to re-use code!

First: outsource the logic for where-clauses. Static expressions are suitable for this, here the example for loading an entity based on an id:

```csharp
public static class MyEntityQuery
{
    public static Expression<Func<MyEntity, bool>> WithId(MyEntityId id)
        => e => e.Id == id;
}
```
See [MyEntityQuery](./src/EntityFrameworkDemo/Database/Queries/MyEntityQuery.cs)

I could also extend this with logic like
```csharp
public static class MyEntityQuery
{
    public static Expression<Func<MyEntity, bool>> IsActive()
        => e => e.ActivatedOn != null;
}
```
This gives me a central place with static methods that I could use in all the places where I want this data filter. Likewise, this method can be covered very very easily in a unit test without needing any dependencies to a repository or to a DB context.

Second: to further centralize the use of queries, it is a good idea to move this query to the repository.

```csharp
public class MyEntityRepository : BaseRepository<MyEntity>
{
    public MyEntityRepository(IMyDemoDbContext dbContext)
        : base(dbContext, dbContext.MyEntities) { }

    // Query
    public IQueryable<MyEntity> QueryById(MyEntityId id, DbTrackingOptions to)
        => Query(to).Where(MyEntityQuery.WithId(id)); 
```
See [MyEntityRepository](./src/EntityFrameworkDemo/Database/Repositories/MyEntityRepository.cs)

So my `QueryById` query now satisfies the requirement that I have to worry zero about what happens under the hood when I use it: I get the entire filter and I can even use this method to enable more filters and have enormously maintainable code.

```csharp
public IQueryable<MyEntity> QueryActiveById(MyEntityId id, DbTrackingOptions to)
    => QueryById(to).Where(MyEntityQuery.IsActive());  // uses QueryById which uses Query
```

> All these query expressions and static filter implementations can be tested very efficiently and easily with unit tests. This reduces the effort and costs for integration tests enormously.


## Projections and Profiles

Most applications have an enormously high read factor (like >99%), but rarely write data. And even more apps need in the particular situation - when data is read - either not the whole entity, or several data from several entities (joins).\
Projections help us to model this into classes and thus load only data that we need.

The representation of a projection is nothing other than a class with properties. In contrast to entities, the implementation as a `record class` is suitable here, because one of the basic ideas is that projections should only ever be created via the constructor so that no properties are forgotten during mappings.

In this example, we have a projection that should only represent a single property of an entity.

```csharp
public sealed record class MyEntityNameProjection(string Name);
```

> The code and runtime-overhead of the additional class is intentionally accepted. Even in very performance-critical situations, the advantage outweighs the disadvantage enormously.

The source is our entity:

```csharp
public sealed class MyEntity : BaseEntity
{
    // There are certain EF Core features (e.g. testing) that always 
    //   require an empty constructor.
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public MyEntity() { }

    // Default constructor to create our entity
    public MyEntity(MyEntityId id, string name)
    {
        Id = id;
        Name = name;
    }

    // our sample properties

    public MyEntityId Id { get; set; } = null!; // is using our custom Id Type!

    [MaxLength(4000)]
    [Unicode(true)]
    public string Name { get; set; } = null!;
}
```

It is now very common to have a simple select that returns a string:

```csharp
List<string> activeEntities = myEntities
                .Select(entity => entity.Name)
                .ToListAsync(ct);
```

I therefore have a responsibility to transform the representation in my calling class, the repository or the logic. This is relatively complex, has a relatively high error potential due to stringification and is cumbersome to test. Furthermore, the maintainability is not very high if you have this in several places.



Projections are now a way of removing this responsibility from the data operation.

The first step is to use our projection like

```csharp
List<MyEntityNameProjection> activeEntities = myEntities
                .Select(entity => new MyEntityNameProjection(entity.Name))
                .ToListAsync(ct);
```

which offers at least a few advantages, but becomes very complex if not only a single property is read, as in this example, but more complex scenarios such as joins and the like. The only advantage here is the typing by the class.

The actual advantage, which eliminates almost all disadvantages, is now the profile of a projection, i.e. the definition based on the source (MyEntity) of how the projection is structured. In this case, it looks like this (see [MyEntityNameProjection.cs](./src/EntityFrameworkDemo/Database/Projections/MyEntityNameProjection.cs)):

```csharp
public static class MyEntityNameProjectionProfiles
{
    // The actual conversion
    private static Expression<Func<MyEntity, MyEntityNameProjection>> MyEntityNameProjection()
        => e => new MyEntityNameProjection(e.Name);

    // The Expandable
    [Expandable(nameof(MyEntityNameProjection))]
    public static MyEntityNameProjection FromMyEntity(MyEntity model)
        => s_myEntityNameProjection.Invoke(model);

    // The compiled state to reduce the runtime load
    private static readonly Func<MyEntity, MyEntityNameProjection> s_myEntityNameProjection
        = MyEntityNameProjection().Compile();
}
```

The profile consists (in optimized form) of three components:

- The actual projection (MyEntityNameProjection), i.e. the expression to map from `MyEntity` to `MyEntityNameProjection`.
- A method (FromMyEntity) with which the profile is to be used in the Linq query.
- A func that corresponds to the compiled expression (MyEntityNameProjection) to avoid overhead.

The special feature that makes this profile work in Linq expressions is the `Expandable` attibute. This comes from [LinqKit](https://github.com/scottksmith95/LINQKit) and enables significantly more complex, but also significantly simpler scenarios and implementation options for Linq. It offers functionalities that should actually come from Linq by default...

The use would be

```csharp
List<MyEntityNameProjection> entityNames = myEntities
        .AsExpandable()
        .Select(e => MyEntityNameProjectionProfiles.FromMyEntity(e))
        .ToListAsync(ct);
```

We have thus eliminated all the disadvantages of the previous query, have a powerful profile with high maintainability, high reusability and the ability to make even complex scenarios look very simple in a query.

To avoid having to specify `AsExpandable` in every query, the [BaseRepository](./src/Database/Repositories/BaseRepository.cs) is structured by default so that absolutely every query is always submitted with `AsExpandable` in order to always take advantage of LinqKit in all queries.

```csharp
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
        _expandableSet = entitySet.AsExpandable();
    }

    // ...

    public IQueryable<T> Query(DbTrackingOptions to)
        // In contrast to all other methods, queries
        //   should only be made via the expandable set!
        => _expandableSet.With(to);
```

All other methods in the BaseRepository are designed in such a way that the raw set (`_set`) is never accessed read-only, only write methods.

See [BaseRepository](./src/Database/Repositories/BaseRepository.cs)

## Migrations and DevOps

The migrations in this example are located in an isolated project [EntityFrameworkDemo.Database.SqlServer.Migrations](./src/EntityFrameworkDemo.Database.SqlServer.Migrations/).

In principle, this is the [recommended way](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/projects?WT.mc_id=DT-MVP-5001507) to deal with EF Core migrations, even if this is not mentioned in the documentation. Unfortunately, the documentation is also very inadequate at this point and not worthy of the product.

More about the migrations here: [EFCore Migrations readme.md](./src/EntityFrameworkDemo.Database.SqlServer.Migrations/readme.md)

## About

This repository is based on my personal best practices and has been used in many of my private and many customer projects for years.

📫 You can reach me via [twitter.com/abt_benjamin](https://twitter.com/abt_benjamin) or [LinkedIn](https://www.linkedin.com/in/benjaminabt/)

<a href="https://www.buymeacoffee.com/benjaminabt" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png" alt="Buy Me A Coffee" height="30" ></a>

## Feedback

Please use the Issue-Tab for Feedback.
https://github.com/BenjaminAbt/efcore-best-practises

## License

```text
Copyright 2023 BEN ABT

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

```

## Comming Soon

- Unit Test Samples
- Your Feedback