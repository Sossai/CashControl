using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net;
using System.Text;

namespace Consolidation.Worker.HealthCheck
{
    public class HealthCheckServer : BackgroundService
    {
        private readonly HealthCheckService _healthCheckService;

        public HealthCheckServer(
            HealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            using var listener = new HttpListener();

            listener.Prefixes.Add(
                "http://+:5298/");

            listener.Start();

            while (!stoppingToken.IsCancellationRequested)
            {
                var context = await listener.GetContextAsync();

                _ = HandleRequestAsync(
                    context,
                    stoppingToken);
            }
        }

        private async Task HandleRequestAsync(
            HttpListenerContext context,
            CancellationToken cancellationToken)
        {
            var path = context.Request.Url?.AbsolutePath;

            if (path == "/health/live")
            {
                context.Response.StatusCode = 200;

                var response = Encoding.UTF8.GetBytes("Healthy");

                context.Response.ContentType = "text/plain";
                context.Response.ContentLength64 = response.Length;

                await context.Response.OutputStream.WriteAsync(
                    response,
                    cancellationToken);

                context.Response.Close();

                return;
            }

            if (path == "/health/ready")
            {
                var report = await _healthCheckService.CheckHealthAsync(
                    check => check.Name == "masstransit-bus",
                    cancellationToken);

                context.Response.StatusCode =
                    report.Status == HealthStatus.Healthy
                        ? 200
                        : 503;

                var response = Encoding.UTF8.GetBytes(
                    report.Status.ToString());

                context.Response.ContentType = "text/plain";
                context.Response.ContentLength64 = response.Length;

                await context.Response.OutputStream.WriteAsync(
                    response,
                    cancellationToken);

                context.Response.Close();

                return;
            }

            context.Response.StatusCode = 404;

            context.Response.Close();
        }
    }
}
