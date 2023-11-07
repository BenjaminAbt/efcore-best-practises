namespace BenjaminAbt.EntityFrameworkDemo.Database;

// this is a demo class for failed database operation exceptions

public class DatabaseOperationFailedException : Exception
{
    public DatabaseOperationFailedException() : base() { }
}
