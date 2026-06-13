using System;
using System.Collections.Generic;
using System.Linq;
using UnitConversion.Domain.Exceptions;
using UnitConversion.Domain.Services;

namespace UnitConversion.Domain.Converters;

public class LinearUnitConverter : IUnitConverter
{
    private readonly IUnitRegistry _registry;

    public string Category { get; }
    public IEnumerable<string> SupportedUnits => _registry.GetUnitsByCategory(Category).Select(u => u.Name);

    public LinearUnitConverter(string category, IUnitRegistry registry)
    {
        Category = category ?? throw new ArgumentNullException(nameof(category));
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    public bool Supports(string unit)
    {
        if (string.IsNullOrEmpty(unit)) return false;
        var unitDef = _registry.GetUnit(unit);
        return unitDef != null && unitDef.Category.Equals(Category, StringComparison.OrdinalIgnoreCase);
    }

    public double Convert(double value, string fromUnit, string toUnit)
    {
        if (string.IsNullOrEmpty(fromUnit)) throw new ArgumentException("From unit cannot be empty.", nameof(fromUnit));
        if (string.IsNullOrEmpty(toUnit)) throw new ArgumentException("To unit cannot be empty.", nameof(toUnit));

        var fromDef = _registry.GetUnit(fromUnit);
        var toDef = _registry.GetUnit(toUnit);

        if (fromDef == null || !fromDef.Category.Equals(Category, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnsupportedUnitException(fromUnit, Category);
        }
        if (toDef == null || !toDef.Category.Equals(Category, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnsupportedUnitException(toUnit, Category);
        }

        // Convert to base unit: baseValue = value / factor
        double valueInBase = value / fromDef.ConversionFactor;
        // Convert to destination unit: destValue = baseValue * factor
        return valueInBase * toDef.ConversionFactor;
    }
}
