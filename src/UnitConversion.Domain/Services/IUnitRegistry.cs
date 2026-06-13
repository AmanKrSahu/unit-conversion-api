using System.Collections.Generic;
using UnitConversion.Domain.Models;

namespace UnitConversion.Domain.Services;

public interface IUnitRegistry
{
    IEnumerable<UnitDefinition> GetUnits();
    IEnumerable<UnitDefinition> GetUnitsByCategory(string category);
    UnitDefinition? GetUnit(string unitName);
}
