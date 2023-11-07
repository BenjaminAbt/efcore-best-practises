using BenjaminAbt.EntityFrameworkDemo.Database;
using BenjaminAbt.EntityFrameworkDemo.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace BenjaminAbt.EntityFrameworkDemo;

// This is the general db context interface for our application.
// With the help of this interface it is possible to change the database provider statically as well as dynamically
//    without having to customize the logic stuff.

public interface IMyDemoDbContext : IBaseDbContext
{
    DbSet<MyEntity> MyEntities { get; }
}

// In this case it is a generic context, which works in this form with "all" EF Core databases.
// If specific DBMS requirements exist, this context must derive from the corresponding DB context.

// Likewise, it is conceivable that multiple context interfaces
//   exist to be able to implement a multi-DB setup at runtime.
