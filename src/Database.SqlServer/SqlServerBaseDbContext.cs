using Microsoft.EntityFrameworkCore;

namespace BenjaminAbt.EntityFrameworkDemo.Database.SqlServer;

public abstract class SqlServerBaseDbContext : BaseDbContext, ISqlServerDbContext
{
    protected SqlServerBaseDbContext(DbContextOptions options) : base(options)
    {
    }
}
