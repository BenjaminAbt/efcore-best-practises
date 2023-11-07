namespace BenjaminAbt.EntityFrameworkDemo.Models;

public interface IPlatformIdentifierGuid<TId> : IPlatformIdentifier<TId, Guid>
    where TId : IPlatformIdentifier<TId, Guid>
{
}
