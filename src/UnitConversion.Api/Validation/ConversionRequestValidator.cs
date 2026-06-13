using System;
using UnitConversion.Api.Models;

namespace UnitConversion.Api.Validation;

public static class ConversionRequestValidator
{
    public static void Validate(ConversionRequestDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentException("Request body or parameters cannot be null.");
        }

        if (dto.Value == null)
        {
            throw new ArgumentException("The 'value' parameter is required.");
        }

        if (double.IsNaN(dto.Value.Value) || double.IsInfinity(dto.Value.Value))
        {
            throw new ArgumentException("The 'value' must be a finite number.");
        }

        if (string.IsNullOrWhiteSpace(dto.From))
        {
            throw new ArgumentException("The 'from' unit parameter is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.To))
        {
            throw new ArgumentException("The 'to' unit parameter is required.");
        }
    }
}
