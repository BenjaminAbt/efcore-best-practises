using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BenjaminAbt.EntityFrameworkDemo.Database.SqlServer.Migrations;

// The SqlServerMigrationContextFactory class is the basis to enable migrations via
// a separate project setup.

// It implies IDesignTimeDbContextFactory, which uses the EF tooling to create migrations.
// This makes it very easy to integrate EF migrations into a modern SaaS DevOps.

public abstract class SqlServerMigrationContextFactory<TDbContext>
    : IDesignTimeDbContextFactory<TDbContext> where TDbContext : DbContext, ISqlServerDbContext
{
    /// <summary>
    /// The assembly in which the migrations are located. 
    /// In our setup, this is the same project that will inherit from this factory later.
    /// </summary>
    public abstract string MigrationAssembly { get; }

    public TDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<TDbContext> optionsBuilder = new();
        optionsBuilder.UseSqlServer(b => b.MigrationsAssembly(MigrationAssembly));

        return (TDbContext)Activator.CreateInstance(typeof(TDbContext), optionsBuilder.Options)!;
    }
}
