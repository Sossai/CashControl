using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;

namespace Transactions.Infrastructure
{
    public class RabbitMqHealthCheck : IHealthCheck
    {
        private readonly IConfiguration _configuration;

        public RabbitMqHealthCheck(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _configuration["RabbitMq:Host"] ?? "localhost",
                    UserName = "cashcontrol",
                    Password = "cashcontrol"
                };

                await using var connection =
                    await factory.CreateConnectionAsync(
                        cancellationToken);

                return HealthCheckResult.Healthy(
                    "RabbitMQ is reachable.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(
                    "RabbitMQ is unavailable.",
                    ex);
            }
        }
    }
}
