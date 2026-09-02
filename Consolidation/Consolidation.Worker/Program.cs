using Consolidation.Application;
using Consolidation.Application.Interfaces;
using Consolidation.Domain.Interfaces;
using Consolidation.Infrastructure;
using Consolidation.Infrastructure.Interfaces;
using Consolidation.Infrastructure.Repository;
using Consolidation.Worker.Consumer;
using Consolidation.Worker.HealthCheck;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;


var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddScoped<IDailyConsolidateRepository, DailyConsolidateRepository>();
builder.Services.AddScoped<IProcessedEventRepository, ProcessedEventRepository>();
builder.Services.AddScoped<IConsolidateManager, ConsolidateManager>();
builder.Services.AddScoped<IProcessedEventRepository, ProcessedEventRepository>();
builder.Services.AddScoped<IConsolidateUnitOfWork, ConsolidateUnitOfWork>();

var rabbitMqHost = builder.Configuration["RabbitMq:Host"] ?? "localhost";

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<TransactionConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(
            rabbitMqHost,
            "/",
            h =>
            {
                h.Username("cashcontrol");
                h.Password("cashcontrol");
            });

        cfg.ReceiveEndpoint(
            "transaction-created",
            e =>
            {
                e.ConfigureConsumer<TransactionConsumer>(context);
            });
    });
});

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

builder.Services.AddHostedService<HealthCheckServer>();
builder.Services.AddHealthChecks();

var host = builder.Build();

host.Run();
