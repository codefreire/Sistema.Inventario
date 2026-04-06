using System.Net;
using System.Text;
using Polly;
using Polly.Extensions.Http;

using Sistema.Inventario.Transaccion.Infraestructura.ClientesExternos;

namespace Sistema.Inventario.Transaccion.API.Extensiones;

/// <summary>
/// Define las politicas de resiliencia HTTP usadas para la comunicación con el cliente de Productos
/// </summary>
public static class PoliticasResilienciaHttp
{
    /// <summary>
    /// Obtiene la politica de resiliencia de fallback para operaciones de lectura (GET)
    /// </summary>
    /// <param name="logger">Logger para registrar eventos de resiliencia</param>
    /// <returns>Politica de resiliencia de fallback para operaciones de lectura</returns>
    public static IAsyncPolicy<HttpResponseMessage> ObtenerPoliticaFallbackLectura(ILogger<ProductoApiCliente> logger)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TaskCanceledException>()
            .OrResult(response => response.StatusCode == HttpStatusCode.TooManyRequests)
            .FallbackAsync(
                fallbackAction: _ =>
                {
                    HttpResponseMessage fallbackResponse = new(HttpStatusCode.ServiceUnavailable)
                    {
                        Content = new StringContent("Fallback de lectura aplicado por resiliencia.", Encoding.UTF8, "text/plain")
                    };
                    return Task.FromResult(fallbackResponse);
                },
                onFallbackAsync: outcome =>
                {
                    logger.LogWarning($"Resilience fallback activado para operacion de lectura. StatusCode: {outcome.Result?.StatusCode}. Excepcion: {outcome.Exception?.GetType().Name}");
                    return Task.CompletedTask;
                });
    }

    /// <summary>
    /// Obtiene la politica de resiliencia de retry
    /// </summary>
    /// <param name="logger">Logger para registrar eventos de resiliencia</param>
    /// <returns>Politica de resiliencia de retry</returns>
    public static IAsyncPolicy<HttpResponseMessage> ObtenerPoliticaRetry(ILogger<ProductoApiCliente> logger)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TaskCanceledException>()
            .OrResult(response => response.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: intento => TimeSpan.FromSeconds(Math.Pow(2, intento)),
                onRetry: (outcome, delay, intento, _) =>
                {
                    logger.LogWarning($"Resilience retry ejecutado. Intento: {intento}. EsperaMs: {delay.TotalMilliseconds}. StatusCode: {outcome.Result?.StatusCode}. Excepcion: {outcome.Exception?.GetType().Name}");
                });
    }

    /// <summary>
    /// Obtiene la politica de resiliencia de circuit breaker
    /// </summary>
    /// <param name="logger">Logger para registrar eventos de resiliencia</param>
    /// <returns>Politica de resiliencia de circuit breaker</returns>
    public static IAsyncPolicy<HttpResponseMessage> ObtenerPoliticaCircuitBreaker(ILogger<ProductoApiCliente> logger)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(response => response.StatusCode == HttpStatusCode.TooManyRequests)
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (outcome, breakDelay) =>
                {
                    logger.LogError($"Circuit breaker abierto. DuracionSeg: {breakDelay.TotalSeconds}. StatusCode: {outcome.Result?.StatusCode}. Excepcion: {outcome.Exception?.GetType().Name}");
                },
                onReset: () =>
                {
                    logger.LogInformation("Circuit breaker restablecido.");
                },
                onHalfOpen: () =>
                {
                    logger.LogInformation("Circuit breaker en estado half-open.");
                });
    }
}
