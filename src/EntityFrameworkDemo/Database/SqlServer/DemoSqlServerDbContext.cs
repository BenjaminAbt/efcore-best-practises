using BenjaminAbt.EntityFrameworkDemo.Database.Converters;
using BenjaminAbt.EntityFrameworkDemo.Database.Entities;
using BenjaminAbt.EntityFrameworkDemo.Database.SqlServer.Configs;
using Microsoft.EntityFrameworkCore;

namespace BenjaminAbt.EntityFrameworkDemo.Database.SqlServer;


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
