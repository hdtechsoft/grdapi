
using System.Threading;
using System.Threading.Tasks;
using grdApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace grdApi.HealthChecks
{
    
    /// <summary>
    /// Checks whether the EF Core DbContext can connect to the database.
    /// Uses Database.CanConnectAsync(), which executes a lightweight ping.
    /// </summary>
    
public class DbConnectionHealthCheck : IHealthCheck
    {
        private readonly AppDbContext _db;

        public DbConnectionHealthCheck(AppDbContext db) => _db = db;

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var canConnect = await _db.Database.CanConnectAsync(cancellationToken);
                return canConnect
                    ? HealthCheckResult.Healthy("Database connection OK.")
                    : HealthCheckResult.Unhealthy("Cannot connect to the database.");
            }
            catch (Exception ex)
            {
                // Include exception details in the health result
                return HealthCheckResult.Unhealthy("Database check failed.", ex);
            }
        }
    }

}
