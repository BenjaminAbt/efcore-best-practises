using BenjaminAbt.EntityFrameworkDemo.Database.Entities;
using BenjaminAbt.EntityFrameworkDemo.Database.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BenjaminAbt.EntityFrameworkDemo.Database.SqlServer.Repositories;

/// <summary>
/// Base repository for SQL Server database.
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public abstract class SqlServerBaseRepository<T>
    : BaseRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Database context of SQL Server.
    /// </summary>
    protected new ISqlServerDbContext DbContext { get; }

    /// <summary>
    /// Constructor for SQL Server base repository.
    /// </summary>
    /// <param name="dbContext">The database context for the repository.</param>
    /// <param name="entitySet">The DbSet containing the entities being accessed.</param>
    protected SqlServerBaseRepository(ISqlServerDbContext dbContext, DbSet<T> entitySet)
        : base(dbContext, entitySet)
    {
        DbContext = dbContext;
    }
}
