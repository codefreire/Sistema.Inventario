using System.Diagnostics;

namespace Sistema.Inventario.Producto.API.Middlewares;

/// <summary>
/// Middleware para medir y registrar el tiempo de procesamiento de cada request HTTP
/// </summary>
public class TiempoRequestMiddleware
{
    /// <summary>
    /// Siguiente middleware del pipeline HTTP
    /// </summary>
    private readonly RequestDelegate _next;

    /// <summary>
    /// Servicio de logging para registrar la duracion de cada request
    /// </summary>
    private readonly ILogger<TiempoRequestMiddleware> _logger;

    /// <summary>
    /// Constructor del middleware de tiempo de request
    /// </summary>
    /// <param name="next">Siguiente middleware del pipeline HTTP</param>
    /// <param name="logger">Logger para registrar eventos del middleware</param>
    public TiempoRequestMiddleware(RequestDelegate next, ILogger<TiempoRequestMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Ejecuta el middleware, mide la duración del request y registra el resultado en logs
    /// </summary>
    /// <param name="context">Contexto HTTP actual</param>
    public async Task InvokeAsync(HttpContext context)
    {
        long inicio = Stopwatch.GetTimestamp();
        await _next(context);
        TimeSpan duracion = Stopwatch.GetElapsedTime(inicio);
        _logger.LogInformation($"HTTP {context.Request.Method} {context.Request.Path} => {context.Response.StatusCode} en {duracion.TotalMilliseconds} ms");
    }
}

/// <summary>
/// Clase de extensiones para registrar el middleware de tiempo de request en el pipeline
/// </summary>
public static class TiempoRequestMiddlewareExtensions
{
    /// <summary>
    /// Agrega el middleware de tiempo de request al pipeline HTTP
    /// </summary>
    /// <param name="appBuilder">Aplicacion HTTP</param>
    /// <returns>Builder de la aplicacion</returns>
    public static IApplicationBuilder UseTiempoRequest(this IApplicationBuilder appBuilder)
    {
        return appBuilder.UseMiddleware<TiempoRequestMiddleware>();
    }
}