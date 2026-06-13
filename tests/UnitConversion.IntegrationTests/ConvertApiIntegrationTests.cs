using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using UnitConversion.Api.Models;
using UnitConversion.Domain.Models;
using Xunit;

namespace UnitConversion.Tests;

public class ConvertApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ConvertApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Convert_ValidRequest_ReturnsOkWithResult()
    {
        // Act
        var response = await _client.GetAsync("/api/convert?value=10&from=meters&to=feet");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.True(HttpStatusCode.OK == response.StatusCode, $"Expected OK but got {response.StatusCode}. Response content: {content}");
        var result = await response.Content.ReadFromJsonAsync<ConversionResult>();
        Assert.NotNull(result);
        Assert.Equal("Length", result.Category);
        Assert.Equal(10, result.OriginalValue);
        Assert.Equal("meters", result.FromUnit);
        Assert.Equal("feet", result.ToUnit);
        Assert.True(result.ConvertedValue > 32.8 && result.ConvertedValue < 32.9);
    }

    [Fact]
    public async Task Convert_InvalidUnit_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/convert?value=10&from=invalidunit&to=feet");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var errorObj = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.NotNull(errorObj);
        Assert.True(errorObj.ContainsKey("error"));
        Assert.Contains("not supported", errorObj["error"]);
    }

    [Fact]
    public async Task Convert_MissingParams_ReturnsBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/convert?value=10&from=&to=feet");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errorObj = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.NotNull(errorObj);
        Assert.True(errorObj.ContainsKey("error"));
        Assert.Contains("required", errorObj["error"]);
    }

    [Fact]
    public async Task Convert_InfiniteValue_ReturnsBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/convert?value=NaN&from=meters&to=feet");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errorObj = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.NotNull(errorObj);
        Assert.True(errorObj.ContainsKey("error"));
        Assert.Contains("finite number", errorObj["error"]);
    }

    [Fact]
    public async Task GetCategories_ReturnsOkWithList()
    {
        // Act
        var response = await _client.GetAsync("/api/categories");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var categories = await response.Content.ReadFromJsonAsync<List<string>>();
        Assert.NotNull(categories);
        Assert.Contains("Length", categories);
        Assert.Contains("Weight", categories);
        Assert.Contains("Temperature", categories);
    }

    [Fact]
    public async Task GetUnits_ValidCategory_ReturnsOkWithFilteredList()
    {
        // Act
        var response = await _client.GetAsync("/api/units?category=Length");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var units = await response.Content.ReadFromJsonAsync<List<string>>();
        Assert.NotNull(units);
        Assert.Contains("meters", units);
        Assert.Contains("feet", units);
    }

    [Fact]
    public async Task GetUnits_NoCategory_ReturnsOkWithAllUnits()
    {
        // Act
        var response = await _client.GetAsync("/api/units");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var units = await response.Content.ReadFromJsonAsync<List<string>>();
        Assert.NotNull(units);
        Assert.Contains("meters", units);
        Assert.Contains("kilograms", units);
        Assert.Contains("celsius", units);
    }

    [Fact]
    public async Task GetHealth_ReturnsOkWithHealthyStatus()
    {
        // Act
        var response = await _client.GetAsync("/api/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var report = await response.Content.ReadFromJsonAsync<HealthResponseDto>();
        Assert.NotNull(report);
        Assert.Equal("Healthy", report.Status);
        Assert.NotNull(report.SystemMetrics);
        Assert.True(report.SystemMetrics.MemoryUsageMb > 0);
        Assert.False(string.IsNullOrWhiteSpace(report.SystemMetrics.OperatingSystem));
        Assert.Contains("unit_registry", report.Entries.Keys);
        Assert.Equal("Healthy", report.Entries["unit_registry"].Status);
    }
}
