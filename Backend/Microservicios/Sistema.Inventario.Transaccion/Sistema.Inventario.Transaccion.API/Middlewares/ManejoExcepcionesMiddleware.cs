namespace Sistema.Inventario.Transaccion.API.Middlewares;

/// <summary>
/// Middleware para mapear excepciones de negocio a respuestas HTTP consistentes
/// </summary>
public class ManejoExcepcionesMiddleware
{
    /// <summary>
    /// Siguiente middleware del pipeline HTTP
    /// </summary>
    private readonly RequestDelegate _next;

    /// <summary>
    /// Servicio de logging para registrar la duración de cada request
    /// </summary>
    private readonly ILogger<ManejoExcepcionesMiddleware> _logger;

    /// <summary>
    /// Constructor del middleware de manejo de excepciones
    /// </summary>
    /// <param name="next">Siguiente middleware del pipeline HTTP</param>
    /// <param name="logger">Logger para registrar excepciones controladas</param>
    public ManejoExcepcionesMiddleware(RequestDelegate next, ILogger<ManejoExcepcionesMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Intercepta excepciones controladas y responde con el código HTTP correspondiente
    /// </summary>
    /// <param name="context">Contexto HTTP actual</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error de negocio en request: {Mensaje}", ex.Message);

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "text/plain; charset=utf-8";
            await context.Response.WriteAsync(ex.Message);
        }
    }
}

/// <summary>
/// Extensiones para registrar el middleware de manejo de excepciones
/// </summary>
public static class ManejoExcepcionesMiddlewareExtensions
{
    /// <summary>
    /// Agrega el middleware de manejo de excepciones al pipeline HTTP
    /// </summary>
    /// <param name="appBuilder">Aplicación HTTP</param>
    /// <returns>Builder de la aplicación</returns>
    public static IApplicationBuilder UseManejoExcepciones(this IApplicationBuilder appBuilder)
    {
        return appBuilder.UseMiddleware<ManejoExcepcionesMiddleware>();
    }
}
