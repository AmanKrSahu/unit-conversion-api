using System;
using System.Collections.Generic;
using System.Linq;
using UnitConversion.Domain.Converters;
using UnitConversion.Domain.Exceptions;
using UnitConversion.Domain.Models;

namespace UnitConversion.Domain.Services;

public class ConversionService : IConversionService
{
    private readonly IEnumerable<IUnitConverter> _converters;
    private readonly IUnitRegistry _registry;

    public ConversionService(IEnumerable<IUnitConverter> converters, IUnitRegistry registry)
    {
        _converters = converters ?? throw new ArgumentNullException(nameof(converters));
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    public ConversionResult Convert(double value, string fromUnit, string toUnit)
    {
        if (string.IsNullOrEmpty(fromUnit)) throw new ArgumentException("From unit cannot be empty.", nameof(fromUnit));
        if (string.IsNullOrEmpty(toUnit)) throw new ArgumentException("To unit cannot be empty.", nameof(toUnit));

        fromUnit = fromUnit.Trim();
        toUnit = toUnit.Trim();

        // 1. Validation check - do they exist in the registry?
        var fromDef = _registry.GetUnit(fromUnit);
        if (fromDef == null)
        {
            throw new UnsupportedUnitException(fromUnit);
        }

        var toDef = _registry.GetUnit(toUnit);
        if (toDef == null)
        {
            throw new UnsupportedUnitException(toUnit);
        }

        // 2. Optimization check - same unit conversion
        if (fromUnit.Equals(toUnit, StringComparison.OrdinalIgnoreCase))
        {
            return new ConversionResult(
                OriginalValue: value,
                FromUnit: fromUnit,
                ConvertedValue: value,
                ToUnit: toUnit,
                Category: fromDef.Category
            );
        }

        // 3. Category match check - can't convert cross categories
        if (!fromDef.Category.Equals(toDef.Category, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidConversionException($"Cannot convert between different categories: '{fromDef.Category}' and '{toDef.Category}'.");
        }

        // 4. Find the matching converter
        var converter = _converters.FirstOrDefault(c => c.Category.Equals(fromDef.Category, StringComparison.OrdinalIgnoreCase));
        if (converter == null)
        {
            throw new InvalidConversionException($"No converter found registered for category '{fromDef.Category}'.");
        }

        double result = converter.Convert(value, fromUnit, toUnit);

        return new ConversionResult(
            OriginalValue: value,
            FromUnit: fromUnit,
            ConvertedValue: result,
            ToUnit: toUnit,
            Category: converter.Category
        );
    }

    public IEnumerable<string> GetSupportedCategories()
    {
        return _converters.Select(c => c.Category).Distinct();
    }

    public IEnumerable<string> GetSupportedUnits(string? category = null)
    {
        // If no category is specified, return all units across all converters
        if (string.IsNullOrWhiteSpace(category))
        {
            return _converters.SelectMany(c => c.SupportedUnits).Distinct();
        }

        // Otherwise, fetch units registered for the requested category
        var converter = _converters.FirstOrDefault(c => c.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        return converter != null ? converter.SupportedUnits : Enumerable.Empty<string>();
    }
}
