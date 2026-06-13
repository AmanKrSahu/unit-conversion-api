using System;

namespace UnitConversion.Domain.Exceptions;

public class InvalidConversionException : Exception
{
    public InvalidConversionException(string message) : base(message)
    {
    }

    public InvalidConversionException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
