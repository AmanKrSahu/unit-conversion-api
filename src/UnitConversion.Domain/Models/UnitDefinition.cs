namespace UnitConversion.Domain.Models;

public record UnitDefinition(
    string Name,
    string Category,
    double ConversionFactor,
    bool IsBaseUnit = false
);
