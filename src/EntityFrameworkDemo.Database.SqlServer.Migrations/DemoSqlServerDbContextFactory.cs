namespace BenjaminAbt.EntityFrameworkDemo.Database.SqlServer.Migrations;

public class DemoSqlServerDbContextFactory
    : SqlServerMigrationContextFactory<DemoSqlServerDbContext>
{
    // the current assembly is the migration assembly!
    public override string MigrationAssembly =>
        typeof(DemoSqlServerDbContextFactory).Assembly.FullName
            ?? throw new Exception("The migration assembly could not be determined!");
}
