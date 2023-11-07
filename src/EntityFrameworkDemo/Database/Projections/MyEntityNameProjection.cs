namespace BenjaminAbt.EntityFrameworkDemo.Database.Projections;

// Projections represent a section of one or more entities or DB queries.
// They are implemented as separate records to be able to manage and use them very easily through profiles.

// Projections are a measure of database performance optimization: only the data that is really needed is loaded.
// They represent a read-only way.

public sealed record class MyEntityNameProjection(string Name);
