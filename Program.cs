using grdApi.Data;
using grdApi.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;



var builder = WebApplication.CreateBuilder(args);


// 1) Define a named CORS policy that matches your Angular dev origin exactly


const string AllowAngular = "AllowAngular";
builder.Services.AddCors(options =>
{
    options.AddPolicy(AllowAngular, p =>
        p.WithOrigins("http://localhost:4200")
         .AllowAnyHeader()
         .AllowAnyMethod());
});
builder.Services.AddControllers();




// EF Core with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// MVC Controllers
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Health Checks
builder.Services.AddHealthChecks()
    // Basic liveness check (always healthy if app is running)
    .AddCheck("self", () => HealthCheckResult.Healthy("App is running."), tags: new[] { "live" })
    // Custom readiness check for DB connectivity
    .AddCheck<DbConnectionHealthCheck>("sql-server", tags: new[] { "ready", "db" });



var app = builder.Build();
app.UseCors(AllowAngular);


// HTTP pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// If you plan to add auth later, also add:
// builder.Services.AddAuthorization();
// app.UseAuthorization();

app.MapControllers(); // ✅ Required for controller routes


// Health endpoints
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = WriteJsonResponse
});

app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = (check) => check.Tags.Contains("ready"),
    ResponseWriter = WriteJsonResponse
});

app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    // Only the 'self' liveness check
    Predicate = (check) => check.Tags.Contains("live"),
    ResponseWriter = WriteJsonResponse
});


app.Run();




// JSON writer for health check responses
static Task WriteJsonResponse(HttpContext context, HealthReport report)
{
    context.Response.ContentType = "application/json; charset=utf-8";

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
            data = e.Value.Data, // additional key/value data if any
            exception = e.Value.Exception?.Message
        })
    };

    var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
    {
        WriteIndented = true
    });

    return context.Response.WriteAsync(json);
}

