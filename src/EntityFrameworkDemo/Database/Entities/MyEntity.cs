using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using BenjaminAbt.EntityFrameworkDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace BenjaminAbt.EntityFrameworkDemo.Database.Entities;

// You cannot use records today
//  https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/records?WT.mc_id=DT-MVP-5001507#immutability

public sealed class MyEntity : BaseEntity
{
    // There are certain EF Core features (e.g. testing) that always 
    //   require an empty constructor.
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public MyEntity() { }

    // Default constructor to create our entity
    public MyEntity(MyEntityId id, string name)
    {
        Id = id;
        Name = name;
    }

    // our sample properties

    public MyEntityId Id { get; set; } = null!; // is using our custom Id Type!

    [MaxLength(4000)]
    [Unicode(true)]
    public string Name { get; set; } = null!;
}
