using BenjaminAbt.EntityFrameworkDemo.Database.Converters;
using BenjaminAbt.EntityFrameworkDemo.Database.Entities;
using BenjaminAbt.EntityFrameworkDemo.Database.Sqlite.Configs;
using BenjaminAbt.EntityFrameworkDemo.Database.Sqlite.Converters;
using BenjaminAbt.EntityFrameworkDemo.Database.SqlServer.Configs;
using Microsoft.EntityFrameworkCore;

namespace BenjaminAbt.EntityFrameworkDemo.Database.SqlServer;

public class DemoMultiDbContext : BaseDbContext, IMyDemoDbContext
{
    public DemoMultiDbContext(DbContextOptions<DemoMultiDbContext> options)
        : base(options) { }

    public DbSet<MyEntity> MyEntities => Set<MyEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // we have to apply OnModelCreating
        base.OnModelCreating(modelBuilder);

        // apply general EF Core conversions
        EFCoreConversions.Apply(modelBuilder);

        if (Database.IsSqlite())
        {
            // apply sqlite server config
            SqliteConversions.Apply(modelBuilder);

            // apply custom sqlite entity configs
            modelBuilder.ApplyConfiguration(new MyEntitySqliteServerConfig());
        }
        else if (Database.IsSqlServer())
        {
            // apply custom sqlserver entity configs
            modelBuilder.ApplyConfiguration(new MyEntitySqlServerConfig());
        }
    }
}
