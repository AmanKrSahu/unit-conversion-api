using System;
using System.Collections.Generic;
using System.Linq;
using UnitConversion.Domain.Models;

namespace UnitConversion.Domain.Services;

public class UnitRegistry : IUnitRegistry
{
    private readonly Dictionary<string, UnitDefinition> _unitsByName;
    private readonly Dictionary<string, List<UnitDefinition>> _unitsByCategory;

    public UnitRegistry(IEnumerable<UnitDefinition> units)
    {
        _unitsByName = new Dictionary<string, UnitDefinition>(StringComparer.OrdinalIgnoreCase);
        _unitsByCategory = new Dictionary<string, List<UnitDefinition>>(StringComparer.OrdinalIgnoreCase);

        foreach (var unit in units)
        {
            var normalizedName = unit.Name.ToLowerInvariant();
            if (_unitsByName.ContainsKey(normalizedName))
            {
                throw new ArgumentException($"Duplicate unit name: '{unit.Name}'");
            }
            _unitsByName[normalizedName] = unit;

            if (!_unitsByCategory.TryGetValue(unit.Category, out var list))
            {
                list = new List<UnitDefinition>();
                _unitsByCategory[unit.Category] = list;
            }
            list.Add(unit);
        }
    }

    public IEnumerable<UnitDefinition> GetUnits() => _unitsByName.Values;

    public IEnumerable<UnitDefinition> GetUnitsByCategory(string category)
    {
        if (_unitsByCategory.TryGetValue(category, out var list))
        {
            return list;
        }
        return Enumerable.Empty<UnitDefinition>();
    }

    public UnitDefinition? GetUnit(string unitName)
    {
        if (string.IsNullOrEmpty(unitName)) return null;
        return _unitsByName.TryGetValue(unitName.ToLowerInvariant(), out var definition) ? definition : null;
    }
}
