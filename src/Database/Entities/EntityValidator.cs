using System.ComponentModel.DataAnnotations;

namespace BenjaminAbt.EntityFrameworkDemo.Database.Entities;

/// <summary>
/// Validator for entities
/// </summary>
public static class EntityValidator
{
    /// <summary>
    /// Tries to validate the entity and returns false if it is invalid, along with a list of validation results.
    /// </summary>
    /// <typeparam name="T">The type of BaseEntity being validated</typeparam>
    /// <param name="entity">The entity to validate</param>
    /// <param name="results">The list of validation results</param>
    /// <returns>True if the entity is valid</returns>
    public static bool TryValidate<T>(T entity, out List<ValidationResult> results) where T : BaseEntity
    {
        ValidationContext validationContext = new(entity);
        results = new List<ValidationResult>();

        return Validator.TryValidateObject(entity, validationContext, results, true);
    }

    /// <summary>
    /// Throws a ValidationException if the entity is invalid.
    /// </summary>
    /// <typeparam name="T">The type of BaseEntity being validated</typeparam>
    /// <param name="entity">The entity to validate</param>
    public static void ThrowIfInvalid<T>(T entity) where T : BaseEntity
    {
        if (!TryValidate(entity, out _))
        {
            throw new ValidationException();
        }
    }
}
