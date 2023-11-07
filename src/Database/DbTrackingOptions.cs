namespace BenjaminAbt.EntityFrameworkDemo.Database;

// DbTrackingOptions are just a flag meant to force when entities
//   are tracked and when you can avoid the overhead of entity tracking

/// <summary>
/// Specifies whether entity tracking is enabled or disabled.
/// </summary>
public enum DbTrackingOptions
{
    /// <summary>
    /// Tracking is enabled.
    /// </summary>
    Enabled,
    /// <summary>
    /// Tracking is disabled.
    /// </summary>
    Disabled
}
