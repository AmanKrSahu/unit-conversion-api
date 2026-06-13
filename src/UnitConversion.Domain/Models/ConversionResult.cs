namespace UnitConversion.Domain.Models;

public record ConversionResult(
    double OriginalValue,
    string FromUnit,
    double ConvertedValue,
    string ToUnit,
    string Category
);
