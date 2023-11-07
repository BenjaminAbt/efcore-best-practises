using System.Linq.Expressions;
using BenjaminAbt.EntityFrameworkDemo.Database.Entities;
using BenjaminAbt.EntityFrameworkDemo.Models;

namespace BenjaminAbt.EntityFrameworkDemo.Database.Queries;

// Static expressions - I always name them queries -  help us to manage certain queries
//   centrally and use them in different places without maintenance effort.
// This reduces the amount of code immensely, simplifies the tests
//   and makes the code much clearer overall.

public static class MyEntityQuery
{
    public static Expression<Func<MyEntity, bool>> WithId(MyEntityId id)
        => e => e.Id == id;
}
