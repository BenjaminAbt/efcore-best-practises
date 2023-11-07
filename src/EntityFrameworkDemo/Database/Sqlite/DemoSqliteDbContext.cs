using BenjaminAbt.EntityFrameworkDemo.Database.Converters;
using BenjaminAbt.EntityFrameworkDemo.Database.Entities;
using BenjaminAbt.EntityFrameworkDemo.Database.Sqlite.Configs;
using Microsoft.EntityFrameworkCore;

namespace BenjaminAbt.EntityFrameworkDemo.Database.Sqlite;

public class DemoSqliteDbContext : SqliteBaseDbContext, IMyDemoDbContext
{
    public DemoSqliteDbContext(DbContextOptions<DemoSqliteDbContext> options)
    : base(options) { }

    public DbSet<MyEntity> MyEntities => Set<MyEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // we have to apply OnModelCreating to get the Sqlite conversions
        base.OnModelCreating(modelBuilder);

        // apply general EF Core conversions
        EFCoreConversions.Apply(modelBuilder);

        // apply sqlite server config
        modelBuilder.ApplyConfiguration(new MyEntitySqliteServerConfig());
    }
}
