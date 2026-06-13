using System;

namespace UnitConversion.Domain.Exceptions;

public class UnsupportedUnitException : Exception
{
    public string Unit { get; }
    public string? Category { get; }

    public UnsupportedUnitException(string unit, string? category = null)
        : base(category != null 
            ? $"Unit '{unit}' is not supported in category '{category}'." 
            : $"Unit '{unit}' is not supported.")
    {
        Unit = unit;
        Category = category;
    }
}
