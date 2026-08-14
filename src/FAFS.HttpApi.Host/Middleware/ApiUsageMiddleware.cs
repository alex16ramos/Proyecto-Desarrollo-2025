using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FAFS.Metrics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace FAFS.Middleware
{
    public class ApiUsageMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiUsageMiddleware> _logger;

        public ApiUsageMiddleware(RequestDelegate next, ILogger<ApiUsageMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? string.Empty;

            // Registrar solo endpoints de la API, excluyendo telemetría propia, health checks y swagger
            if (!path.StartsWith("/api", StringComparison.OrdinalIgnoreCase) ||
                path.Contains("api-usage-metric", StringComparison.OrdinalIgnoreCase) ||
                path.Contains("swagger", StringComparison.OrdinalIgnoreCase) ||
                path.Contains("health-status", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            var stopwatch = Stopwatch.StartNew();
            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                var durationMs = stopwatch.ElapsedMilliseconds;
                var statusCode = context.Response.StatusCode;
                var method = context.Request.Method;
                var clientIp = context.Connection.RemoteIpAddress?.ToString();

                // Guardar métrica en segundo plano de forma desacoplada
                _ = Task.Run(async () =>
                {
                    try
                    {
                        using var scope = context.RequestServices.CreateScope();
                        var metricRepository = scope.ServiceProvider.GetService<IRepository<ApiUsageMetric, Guid>>();
                        var currentUser = scope.ServiceProvider.GetService<ICurrentUser>();

                        if (metricRepository != null)
                        {
                            var metric = new ApiUsageMetric(
                                Guid.NewGuid(),
                                path,
                                method,
                                statusCode,
                                durationMs,
                                currentUser?.Id,
                                clientIp
                            );

                            await metricRepository.InsertAsync(metric, autoSave: true);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error al registrar métrica de uso de API para {Path}.", path);
                    }
                });
            }
        }
    }
}
