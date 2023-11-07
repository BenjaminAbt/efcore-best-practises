using System.Linq.Expressions;
using BenjaminAbt.EntityFrameworkDemo.Database.Entities;
using LinqKit;

namespace BenjaminAbt.EntityFrameworkDemo.Database.Projections.Profiles;

// Projection Profiles are configurations of how to transform from a source to a target.

// This kind of profiles, which are executed by expandables directly in the expression, have the
//   advantage over alternatives like the AutoMapper, that they can also take parameters to
//   use them in the expression (e.g. Skip/Take when paging sub-collections).

// Furthermore, this type of construction allows full compiler support, which does not exist
//   with other types of profiles; e.g. when an additional property is added.
// This reduces the error-proneness, the test effort, but it increases the performance.

public static class MyEntityNameProjectionProfiles
{
    // The actual conversion
    private static Expression<Func<MyEntity, MyEntityNameProjection>> MyEntityNameProjection()
        => e => new MyEntityNameProjection(e.Name);

    // The Expandable
    [Expandable(nameof(MyEntityNameProjection))]
    public static MyEntityNameProjection FromMyEntity(MyEntity model)
        => s_myEntityNameProjection.Invoke(model);

    // The compiled state to reduce the runtime load
    private static readonly Func<MyEntity, MyEntityNameProjection> s_myEntityNameProjection
        = MyEntityNameProjection().Compile();
}
