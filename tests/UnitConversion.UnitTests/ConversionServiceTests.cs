using System;
using System.Collections.Generic;
using UnitConversion.Domain.Converters;
using UnitConversion.Domain.Exceptions;
using UnitConversion.Domain.Models;
using UnitConversion.Domain.Services;
using Xunit;

namespace UnitConversion.Tests;

public class ConversionServiceTests
{
    private readonly ConversionService _conversionService;

    public ConversionServiceTests()
    {
        var units = new List<UnitDefinition>
        {
            // Temperature
            new("celsius", "Temperature", 1.0, IsBaseUnit: true),
            new("fahrenheit", "Temperature", 1.0),
            new("kelvin", "Temperature", 1.0),
            new("c", "Temperature", 1.0, IsBaseUnit: true),
            new("f", "Temperature", 1.0),
            new("k", "Temperature", 1.0),

            // Length
            new("meters", "Length", 1.0, IsBaseUnit: true),
            new("feet", "Length", 3.2808399),
            new("inches", "Length", 39.3700787),

            // Weight
            new("kilograms", "Weight", 1.0, IsBaseUnit: true),
            new("pounds", "Weight", 2.20462262),
            new("grams", "Weight", 1000)
        };

        var registry = new UnitRegistry(units);

        var converters = new List<IUnitConverter>
        {
            new TemperatureConverter(registry),
            new LinearUnitConverter("Length", registry),
            new LinearUnitConverter("Weight", registry)
        };

        _conversionService = new ConversionService(converters, registry);
    }

    [Theory]
    [InlineData(10, "meters", "feet", 32.808399)]
    [InlineData(1, "kilograms", "pounds", 2.20462262)]
    [InlineData(1000, "grams", "kilograms", 1)]
    public void Convert_LinearUnits_ReturnsExpected(double value, string from, string to, double expected)
    {
        var result = _conversionService.Convert(value, from, to);
        Assert.Equal(expected, result.ConvertedValue, precision: 4);
        Assert.Equal(from, result.FromUnit);
        Assert.Equal(to, result.ToUnit);
    }

    [Theory]
    [InlineData(0, "celsius", "fahrenheit", 32)]
    [InlineData(100, "celsius", "fahrenheit", 212)]
    [InlineData(32, "fahrenheit", "celsius", 0)]
    [InlineData(0, "kelvin", "celsius", -273.15)]
    public void Convert_TemperatureUnits_ReturnsExpected(double value, string from, string to, double expected)
    {
        var result = _conversionService.Convert(value, from, to);
        Assert.Equal(expected, result.ConvertedValue, precision: 2);
    }

    [Fact]
    public void Convert_UnsupportedUnit_ThrowsUnsupportedUnitException()
    {
        Assert.Throws<UnsupportedUnitException>(() => 
            _conversionService.Convert(10, "meters", "unsupported_unit"));
    }

    [Fact]
    public void Convert_CrossCategoryUnits_ThrowsInvalidConversionException()
    {
        Assert.Throws<InvalidConversionException>(() => 
            _conversionService.Convert(10, "meters", "kilograms"));
    }

    [Fact]
    public void Convert_NullOrEmptyUnits_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => 
            _conversionService.Convert(10, "", "meters"));
    }

    [Fact]
    public void Convert_SameUnit_ShortCircuitsOptimally()
    {
        var result = _conversionService.Convert(42, "meters", "meters");
        Assert.Equal(42, result.ConvertedValue);
        Assert.Equal("meters", result.FromUnit);
        Assert.Equal("meters", result.ToUnit);
        Assert.Equal("Length", result.Category);
    }
}
