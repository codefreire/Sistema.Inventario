using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Sistema.Inventario.Transaccion.Aplicacion.Handlers;
using Sistema.Inventario.Transaccion.Aplicacion.Servicios;
using Sistema.Inventario.Transaccion.Aplicacion.Validators;
using Sistema.Inventario.Transaccion.Infraestructura.Persistencia;
using Sistema.Inventario.Transaccion.Infraestructura.Repositorios;

namespace Sistema.Inventario.Transaccion.API.Extensiones;

/// <summary>
/// Clase de extensiones para configurar los servicios de la aplicación
/// </summary>
public static class ExtensionesServicios
{
    /// <summary>
    /// Método de extensión para configurar los servicios de la aplicación para las Transacciones
    /// </summary>
    /// <param name="servicios">Colección de servicios de la aplicación</param>
    /// <param name="configuracion">Configuración de la aplicación</param>
    /// <returns>Colección de servicios</returns>
    public static IServiceCollection AddTransacciones(this IServiceCollection servicios, IConfiguration configuracion)
    {
        servicios.AddDbContext<TransaccionDbContext>(opciones =>
            opciones.UseSqlServer(configuracion.GetConnectionString("cnInventarioTransaccionesBD")));

        servicios.AddScoped<ITransaccionRepositorio, TransaccionRepositorio>();
        servicios.AddScoped<ITransaccionServicio, TransaccionServicio>();
        servicios.AddScoped<ObtenerTransaccionesHandler>();
        servicios.AddScoped<ObtenerTransaccionHandler>();
        servicios.AddScoped<CrearTransaccionHandler>();
        servicios.AddScoped<ActualizarTransaccionHandler>();
        servicios.AddScoped<EliminarTransaccionHandler>();

        servicios.AddFluentValidationAutoValidation();
        servicios.AddValidatorsFromAssemblyContaining<ObtenerTransaccionPorIdValidator>();
        servicios.AddValidatorsFromAssemblyContaining<CrearTransaccionValidator>();
        servicios.AddValidatorsFromAssemblyContaining<ActualizarTransaccionValidator>();
        return servicios;
    }
}