using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using UnitConversion.Api.Models;

namespace UnitConversion.Api.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;
    private static readonly DateTime _startTime = DateTime.UtcNow;

    public HealthController(HealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService ?? throw new ArgumentNullException(nameof(healthCheckService));
    }

    /// <summary>
    /// GET /api/health
    /// Runs registered health checks and returns detailed status and system metrics.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HealthResponseDto))]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(HealthResponseDto))]
    public async Task<IActionResult> GetHealth()
    {
        var report = await _healthCheckService.CheckHealthAsync();

        var response = new HealthResponseDto
        {
            Status = report.Status.ToString(),
            TotalDurationMs = report.TotalDuration.TotalMilliseconds,
            Entries = report.Entries.ToDictionary(
                kvp => kvp.Key,
                kvp => new HealthEntryDto
                {
                    Status = kvp.Value.Status.ToString(),
                    Description = kvp.Value.Description ?? string.Empty,
                    Data = kvp.Value.Data.ToDictionary(d => d.Key, d => d.Value)
                }
            ),
            SystemMetrics = new SystemMetricsDto
            {
                Uptime = (DateTime.UtcNow - _startTime).ToString(@"dd\.hh\:mm\:ss"),
                MemoryUsageMb = Math.Round(System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / (1024.0 * 1024.0), 2),
                OperatingSystem = System.Runtime.InteropServices.RuntimeInformation.OSDescription
            }
        };

        var json = System.Text.Json.JsonSerializer.Serialize(response);

        if (report.Status == HealthStatus.Healthy)
        {
            return Content(json, "application/json");
        }

        return StatusCode(StatusCodes.Status503ServiceUnavailable, Content(json, "application/json"));
    }
}
