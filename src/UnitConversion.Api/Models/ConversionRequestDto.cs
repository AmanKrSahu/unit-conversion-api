namespace UnitConversion.Api.Models;

public class ConversionRequestDto
{
    public double? Value { get; set; }
    public string? From { get; set; }
    public string? To { get; set; }
}
