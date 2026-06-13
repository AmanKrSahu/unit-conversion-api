using System.Collections.Generic;
using UnitConversion.Domain.Models;

namespace UnitConversion.Domain.Services;

public interface IConversionService
{
    /// <summary>
    /// Performs numerical value conversion between two units.
    /// </summary>
    ConversionResult Convert(double value, string fromUnit, string toUnit);

    /// <summary>
    /// Gets all registered conversion categories.
    /// </summary>
    IEnumerable<string> GetSupportedCategories();

    /// <summary>
    /// Gets supported unit names, optionally filtered by a specific category.
    /// </summary>
    IEnumerable<string> GetSupportedUnits(string? category = null);
}
