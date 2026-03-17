using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Sistema.Inventario.Producto.Aplicacion.Handlers;
using Sistema.Inventario.Producto.Aplicacion.Servicios;
using Sistema.Inventario.Producto.Aplicacion.Validators;
using Sistema.Inventario.Producto.Infraestructura.Persistencia;
using Sistema.Inventario.Producto.Infraestructura.Repositorios;

namespace Sistema.Inventario.Producto.API.Extensiones;

/// <summary>
/// Clase de extensiones para configurar los servicios de la aplicación
/// </summary>
public static class ExtensionesServicios
{
    /// <summary>
    /// Método de extensión para configurar los servicios de la aplicación para los Productos
    /// </summary>
    /// <param name="servicios">Colección de servicios de la aplicación</param>
    /// <param name="configuracion">Configuración de la aplicación</param>
    /// <returns>Colección de servicios</returns>
    public static IServiceCollection AddProductos(this IServiceCollection servicios, IConfiguration configuracion)
    {
        servicios.AddDbContext<ProductoDbContext>(opciones =>
            opciones.UseSqlServer(configuracion.GetConnectionString("cnInventarioProductosBD")));

        servicios.AddScoped<IProductoRepositorio, ProductoRepositorio>();
        servicios.AddScoped<IProductoServicio, ProductoServicio>();
        servicios.AddScoped<ObtenerProductosHandler>();
        servicios.AddScoped<ObtenerProductoHandler>();
        servicios.AddScoped<CrearProductoHandler>();
        servicios.AddScoped<ActualizarProductoHandler>();
        servicios.AddScoped<EliminarProductoHandler>();

        servicios.AddFluentValidationAutoValidation();
        servicios.AddValidatorsFromAssemblyContaining<ObtenerProductoPorIdValidator>();
        servicios.AddValidatorsFromAssemblyContaining<CrearProductoValidator>();
        servicios.AddValidatorsFromAssemblyContaining<ActualizarProductoValidator>();
        return servicios;
    }
}