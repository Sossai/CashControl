using Consolidation.Application;
using Consolidation.Application.Interfaces;
using Consolidation.Domain.Interfaces;
using Consolidation.Infrastructure;
using Consolidation.Infrastructure.Interfaces;
using Consolidation.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Required for endpoint discovery
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IConsolidateManager, ConsolidateManager>();
builder.Services.AddScoped<IDailyConsolidateRepository, DailyConsolidateRepository>();
builder.Services.AddScoped<IProcessedEventRepository, ProcessedEventRepository>();
builder.Services.AddScoped<IConsolidateUnitOfWork, ConsolidateUnitOfWork>();

builder.Services.AddDbContext<ConsolidatesDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();   // Serves the generated OpenAPI spec as a JSON endpoint
    app.UseSwaggerUI(); // Serves the interactive web UI
}

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
