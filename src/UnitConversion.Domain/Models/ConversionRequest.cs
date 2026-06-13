namespace UnitConversion.Domain.Models;

public record ConversionRequest(double Value, string FromUnit, string ToUnit);
