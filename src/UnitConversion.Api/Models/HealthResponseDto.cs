using System.Collections.Generic;

namespace UnitConversion.Api.Models;

public class HealthResponseDto
{
    public string Status { get; set; } = string.Empty;
    public double TotalDurationMs { get; set; }
    public Dictionary<string, HealthEntryDto> Entries { get; set; } = new();
    public SystemMetricsDto SystemMetrics { get; set; } = new();
}

public class HealthEntryDto
{
    public string Status { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, object> Data { get; set; } = new();
}

public class SystemMetricsDto
{
    public string Uptime { get; set; } = string.Empty;
    public double MemoryUsageMb { get; set; }
    public string OperatingSystem { get; set; } = string.Empty;
}
