
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;
namespace grdApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class HealthController : ControllerBase
    {
        private readonly HealthCheckService _health;
        public HealthController(HealthCheckService health) => _health = health;
        /// <summary>Liveness: process is up.</summary>
        [HttpGet("live")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> Live(CancellationToken ct)
        {
            var report = await _health.CheckHealthAsync(
                check => check.Tags.Contains("live"), ct);

            return ToJson(report);
        }

         /// <summary>Readiness: dependencies (e.g., SQL Server) are reachable.</summary>
        [HttpGet("ready")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> Ready(CancellationToken ct)
        {
            var report = await _health.CheckHealthAsync(
                check => check.Tags.Contains("ready"), ct);

            return ToJson(report);
        }

        /// <summary>Aggregate health status.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> All(CancellationToken ct)
        {
            var report = await _health.CheckHealthAsync(ct);
            return ToJson(report);
        }
        

          private IActionResult ToJson(HealthReport report)
        {
            var payload = new
            {
                status = report.Status.ToString(),
                totalDurationMs = report.TotalDuration.TotalMilliseconds,
                entries = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description,
                    durationMs = e.Value.Duration.TotalMilliseconds,
                    tags = e.Value.Tags,
                    data = e.Value.Data,
                    exception = e.Value.Exception?.Message
                })
            };

            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });

            // Return 200 if Healthy/Degraded, 503 if Unhealthy (typical readiness semantics)
            var statusCode = report.Status == HealthStatus.Unhealthy
                ? StatusCodes.Status503ServiceUnavailable
                : StatusCodes.Status200OK;

            return new ContentResult
            {
                Content = json,
                ContentType = "application/json; charset=utf-8",
                StatusCode = statusCode
            };
        }


    }
}