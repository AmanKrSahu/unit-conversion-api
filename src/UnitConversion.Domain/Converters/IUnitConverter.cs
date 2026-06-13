namespace UnitConversion.Domain.Converters;

public interface IUnitConverter
{
    string Category { get; }
    bool Supports(string unit);
    double Convert(double value, string fromUnit, string toUnit);
    IEnumerable<string> SupportedUnits { get; }
}
