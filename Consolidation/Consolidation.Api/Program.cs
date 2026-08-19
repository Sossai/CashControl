using Consolidation.Application;
using Consolidation.Application.Interfaces;
using Consolidation.Domain.Interfaces;
using Consolidation.Infrastructure;
using Consolidation.Infrastructure.Interfaces;
using Consolidation.Infrastructure.Repository;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services
    .AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "postgres",
        tags: ["ready"]);
    //.AddCheck<RabbitMqHealthCheck>(
    //    "rabbitmq",
    //    tags: ["ready"]);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IConsolidateManager, ConsolidateManager>();
builder.Services.AddScoped<IDailyConsolidateRepository, DailyConsolidateRepository>();
builder.Services.AddScoped<IProcessedEventRepository, ProcessedEventRepository>();
builder.Services.AddScoped<IConsolidateUnitOfWork, ConsolidateUnitOfWork>();

builder.Services.AddDbContext<ConsolidatesDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorCodesToAdd: null);
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();   // Serves the generated OpenAPI spec as a JSON endpoint
    app.UseSwaggerUI(); // Serves the interactive web UI
}

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});

app.MapHealthChecks(
    "/health/ready",
    new HealthCheckOptions
    {
        Predicate = _ => true,

        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType =
                "application/json";

            var response = new
            {
                status = report.Status.ToString(),

                checks = report.Entries.Select(x => new
                {
                    name = x.Key,
                    status = x.Value.Status.ToString(),
                    description = x.Value.Description
                })
            };

            await context.Response.WriteAsJsonAsync(
                response);
        }
    });

app.Run();
