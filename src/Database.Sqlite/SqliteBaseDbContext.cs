using BenjaminAbt.EntityFrameworkDemo.Database.Sqlite.Converters;
using Microsoft.EntityFrameworkCore;

namespace BenjaminAbt.EntityFrameworkDemo.Database.Sqlite;

public abstract class SqliteBaseDbContext : BaseDbContext, ISqliteDbContext
{
    protected SqliteBaseDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply base Sqlite Conversions
        SqliteConversions.Apply(modelBuilder);
    }
}
