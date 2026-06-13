using System;
using System.Collections.Generic;
using System.Linq;
using UnitConversion.Domain.Exceptions;
using UnitConversion.Domain.Services;

namespace UnitConversion.Domain.Converters;

public class TemperatureConverter : IUnitConverter
{
    private readonly IUnitRegistry _registry;

    public string Category => "Temperature";
    public IEnumerable<string> SupportedUnits => _registry.GetUnitsByCategory(Category).Select(u => u.Name);

    public TemperatureConverter(IUnitRegistry registry)
    {
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

        if (!Supports(fromUnit)) throw new UnsupportedUnitException(fromUnit, Category);
        if (!Supports(toUnit)) throw new UnsupportedUnitException(toUnit, Category);

        fromUnit = NormalizeUnit(fromUnit);
        toUnit = NormalizeUnit(toUnit);

        // Convert to Celsius first
        double celsiusValue = fromUnit switch
        {
            "celsius" => value,
            "fahrenheit" => (value - 32.0) * 5.0 / 9.0,
            "kelvin" => value - 273.15,
            _ => throw new InvalidConversionException($"Unexpected source unit '{fromUnit}' for temperature.")
        };

        // Convert from Celsius to toUnit
        return toUnit switch
        {
            "celsius" => celsiusValue,
            "fahrenheit" => (celsiusValue * 9.0 / 5.0) + 32.0,
            "kelvin" => celsiusValue + 273.15,
            _ => throw new InvalidConversionException($"Unexpected target unit '{toUnit}' for temperature.")
        };
    }

    private static string NormalizeUnit(string unit)
    {
        return unit.ToLowerInvariant() switch
        {
            "c" or "celsius" => "celsius",
            "f" or "fahrenheit" => "fahrenheit",
            "k" or "kelvin" => "kelvin",
            _ => unit.ToLowerInvariant()
        };
    }
}
