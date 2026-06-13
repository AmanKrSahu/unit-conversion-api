using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using UnitConversion.Domain.Services;

namespace UnitConversion.Api.HealthChecks;

public class UnitRegistryHealthCheck : IHealthCheck
{
    private readonly IUnitRegistry _registry;

    public UnitRegistryHealthCheck(IUnitRegistry registry)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var units = _registry.GetUnits();
        var data = new Dictionary<string, object>
        {
            { "totalUnits", units.Count() },
            { "totalCategories", units.Select(u => u.Category).Distinct().Count() }
        };

        if (units.Any())
        {
            return Task.FromResult(HealthCheckResult.Healthy("Unit Registry is loaded and ready.", data));
        }

        return Task.FromResult(HealthCheckResult.Unhealthy("Unit Registry is empty or not initialized.", data: data));
    }
}
