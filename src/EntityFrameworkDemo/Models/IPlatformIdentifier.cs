namespace BenjaminAbt.EntityFrameworkDemo.Models;

public interface IPlatformIdentifier<TId, TType> : IComparable<TId>
    where TId : IPlatformIdentifier<TId, TType>
    where TType : IComparable<TType>
{
    TType Value { get; init; }
}
