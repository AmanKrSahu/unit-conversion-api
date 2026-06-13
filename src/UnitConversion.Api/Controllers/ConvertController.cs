using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UnitConversion.Api.Models;
using UnitConversion.Api.Validation;
using UnitConversion.Domain.Models;
using UnitConversion.Domain.Services;

namespace UnitConversion.Api.Controllers;

[ApiController]
[Route("api/convert")]
public class ConvertController : ControllerBase
{
    private readonly IConversionService _conversionService;

    public ConvertController(IConversionService conversionService)
    {
        _conversionService = conversionService ?? throw new ArgumentNullException(nameof(conversionService));
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ConversionResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ConversionResult> Convert([FromQuery] ConversionRequestDto request)
    {
        ConversionRequestValidator.Validate(request);
        
        var result = _conversionService.Convert(request.Value!.Value, request.From!, request.To!);
        var json = System.Text.Json.JsonSerializer.Serialize(result);
        return Content(json, "application/json");
    }

    /// <summary>
    /// GET /api/categories
    /// Retrieves all supported unit categories.
    /// </summary>
    [HttpGet("/api/categories")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<string>))]
    public ActionResult GetCategories()
    {
        var categories = _conversionService.GetSupportedCategories();
        var json = System.Text.Json.JsonSerializer.Serialize(categories);
        return Content(json, "application/json");
    }

    /// <summary>
    /// GET /api/units
    /// Retrieves registered unit names, optionally filtered by category.
    /// </summary>
    [HttpGet("/api/units")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<string>))]
    public ActionResult GetUnits([FromQuery] string? category = null)
    {
        // Fetch matching units from service (returns all if category is omitted)
        var units = _conversionService.GetSupportedUnits(category);
        var json = System.Text.Json.JsonSerializer.Serialize(units);
        return Content(json, "application/json");
    }
}
