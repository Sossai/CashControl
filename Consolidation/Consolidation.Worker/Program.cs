using Consolidation.Application;
using Consolidation.Application.Interfaces;
using Consolidation.Domain.Interfaces;
using Consolidation.Infrastructure;
using Consolidation.Infrastructure.Repository;
using Consolidation.Worker;
using Consolidation.Worker.Consumer;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Domain.Entities;
using System.Transactions;


var builder = Host.CreateApplicationBuilder(args);
//builder.Services.AddHostedService<Worker>();

builder.Services.AddScoped<IDailyConsolidateRepository, DailyConsolidateRepository>();
builder.Services.AddScoped<IConsolidateManager, ConsolidateManager>();


builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<TransactionConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(
            "localhost",
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
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var host = builder.Build();
host.Run();
