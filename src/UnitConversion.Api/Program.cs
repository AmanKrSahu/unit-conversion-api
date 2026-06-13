using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UnitConversion.Api.HealthChecks;
using UnitConversion.Api.Middleware;
using UnitConversion.Domain.Converters;
using UnitConversion.Domain.Models;
using UnitConversion.Domain.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks()
    .AddCheck<UnitRegistryHealthCheck>("unit_registry");

// Define the unit definitions to load into the registry
var units = new List<UnitDefinition>
{
    // Length (Base unit: meters)
    new("m", "Length", 1.0, IsBaseUnit: true),
    new("meters", "Length", 1.0, IsBaseUnit: true),
    new("meter", "Length", 1.0, IsBaseUnit: true),
    new("cm", "Length", 100.0),
    new("centimeters", "Length", 100.0),
    new("centimeter", "Length", 100.0),
    new("mm", "Length", 1000.0),
    new("millimeters", "Length", 1000.0),
    new("kilometer", "Length", 0.001),
    new("kilometers", "Length", 0.001),
    new("km", "Length", 0.001),
    new("inch", "Length", 39.3700787),
    new("inches", "Length", 39.3700787),
    new("in", "Length", 39.3700787),
    new("feet", "Length", 3.2808399),
    new("foot", "Length", 3.2808399),
    new("ft", "Length", 3.2808399),
    new("yard", "Length", 1.0936133),
    new("yards", "Length", 1.0936133),
    new("yd", "Length", 1.0936133),
    new("mile", "Length", 0.000621371),
    new("miles", "Length", 0.000621371),
    new("mi", "Length", 0.000621371),

    // Weight (Base unit: kilograms)
    new("kg", "Weight", 1.0, IsBaseUnit: true),
    new("kilograms", "Weight", 1.0, IsBaseUnit: true),
    new("kilogram", "Weight", 1.0, IsBaseUnit: true),
    new("g", "Weight", 1000.0),
    new("grams", "Weight", 1000.0),
    new("gram", "Weight", 1000.0),
    new("mg", "Weight", 1000000.0),
    new("milligrams", "Weight", 1000000.0),
    new("lb", "Weight", 2.20462262),
    new("lbs", "Weight", 2.20462262),
    new("pounds", "Weight", 2.20462262),
    new("pound", "Weight", 2.20462262),
    new("oz", "Weight", 35.2739619),
    new("ounces", "Weight", 35.2739619),
    new("ounce", "Weight", 35.2739619),
    new("ton", "Weight", 0.00110231),
    new("tons", "Weight", 0.00110231),

    // Temperature (Base unit: celsius, custom math handled in converter)
    new("celsius", "Temperature", 1.0, IsBaseUnit: true),
    new("c", "Temperature", 1.0, IsBaseUnit: true),
    new("fahrenheit", "Temperature", 1.0),
    new("f", "Temperature", 1.0),
    new("kelvin", "Temperature", 1.0),
    new("k", "Temperature", 1.0)
};

// Register the UnitRegistry
builder.Services.AddSingleton<IUnitRegistry>(new UnitRegistry(units));

// Register the unit converters
builder.Services.AddSingleton<IUnitConverter>(sp => new TemperatureConverter(sp.GetRequiredService<IUnitRegistry>()));
builder.Services.AddSingleton<IUnitConverter>(sp => new LinearUnitConverter("Length", sp.GetRequiredService<IUnitRegistry>()));
builder.Services.AddSingleton<IUnitConverter>(sp => new LinearUnitConverter("Weight", sp.GetRequiredService<IUnitRegistry>()));

// Register the main ConversionService
builder.Services.AddSingleton<IConversionService, ConversionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseAuthorization();
app.MapControllers();

app.Run();

// Make the implicit Program class public so test projects can access it via WebApplicationFactory
public partial class Program { }
