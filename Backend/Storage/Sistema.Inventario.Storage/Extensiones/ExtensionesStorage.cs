using Microsoft.Extensions.DependencyInjection;
using Sistema.Inventario.Storage.Servicios;

namespace Sistema.Inventario.Storage.Extensiones;

/// <summary>
/// Extensiones para registrar dependencias del módulo de almacenamiento.
/// </summary>
public static class ExtensionesStorage
{
    /// <summary>
    /// Registra el servicio de almacenamiento en el contenedor de dependencias.
    /// </summary>
    /// <param name="servicios">Colección de servicios de la aplicación.</param>
    /// <returns>Colección de servicios actualizada.</returns>
    public static IServiceCollection AddAlmacenamiento(this IServiceCollection servicios)
    {
        servicios.AddScoped<IAlmacenamientoServicio, AlmacenamientoServicio>();
        return servicios;
    }
}
