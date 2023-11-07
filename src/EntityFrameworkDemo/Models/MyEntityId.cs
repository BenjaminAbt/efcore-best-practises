using System.Diagnostics.CodeAnalysis;

namespace BenjaminAbt.EntityFrameworkDemo.Models;

// This is our custom id, which has a Guid under the hood and implements the appropriate
//    comparison methods so that EF Core can correctly handle our type.

// On top of that, there are a few other methods that make our lives easier.
// In my productive applications I use for automatic generation appropriate
//    source code generators.

// This example here should show the full implementation.

public sealed record class MyEntityId(Guid Value)
    : IPlatformIdentifierGuid<MyEntityId>
{
    // provide optionally a string ctor
    public MyEntityId(string val) : this(Guid.Parse(val)) { }

    // create new Ids during runtime
    public static MyEntityId New() => new(Guid.NewGuid());

    // EF Core wants the CompareTo for internal comparison
    public int CompareTo(MyEntityId? other) => Value.CompareTo(other?.Value);

    // return a string as default like Guid does
    public override string ToString() => Value.ToString();

    // provide a static method for lists
    [return: NotNullIfNotNull(nameof(source))]
    public static List<MyEntityId>? From(IEnumerable<Guid>? source)
        => source?.Select(e => new MyEntityId(e)).ToList();
}
