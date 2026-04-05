using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Sistema.Inventario.Transaccion.Aplicacion.Handlers;
using Sistema.Inventario.Transaccion.Aplicacion.Servicios;
using Sistema.Inventario.Transaccion.Aplicacion.Validators;
using Sistema.Inventario.Transaccion.Infraestructura.ClientesExternos;
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
        // Agregar servicios de Health Checks para el microservicio de Transacciones
        servicios.AddHealthChecks();

        // Agrega DbContext para el microservicio de Transacciones
        servicios.AddDbContext<TransaccionDbContext>(opciones =>
            opciones.UseSqlServer(configuracion.GetConnectionString("cnInventarioTransaccionesBD")));

        // Agrega Swagger para la documentación de la API
        servicios.AddSwaggerGen(opciones =>
        {
            opciones.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "API Microservicio de Transacciones",
                Version = "v1",
                Description = "Microservicio para la gestión de transacciones en el sistema de inventario",
                Contact = new OpenApiContact
                {
                    Name = "Carlos Freire",
                    Email = "freirece@produbanco.com"
                }
            });

            string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            opciones.IncludeXmlComments(xmlPath);
        });

        // Comunicación con el microservicio de Productos via HttpClient
        servicios.AddHttpClient<IProductoApiCliente, ProductoApiCliente>(cliente =>
        {
            string productoApiUrl = configuracion["MicroserviciosUrls:ProductoApi"]
                ?? "http://localhost:5261";
            cliente.BaseAddress = new Uri(productoApiUrl);
        });

        // Registrar repositorios, servicios y handlers del microservicio de Transacciones
        servicios.AddScoped<ITransaccionRepositorio, TransaccionRepositorio>();
        servicios.AddScoped<ITransaccionServicio, TransaccionServicio>();
        servicios.AddScoped<ObtenerTransaccionesHandler>();
        servicios.AddScoped<ObtenerTransaccionHandler>();
        servicios.AddScoped<CrearTransaccionHandler>();
        servicios.AddScoped<ActualizarTransaccionHandler>();
        servicios.AddScoped<EliminarTransaccionHandler>();

        // Configurar FluentValidation para validar las solicitudes de la API
        servicios.AddFluentValidationAutoValidation();
        servicios.AddValidatorsFromAssemblyContaining<ObtenerTransaccionPorIdValidator>();
        servicios.AddValidatorsFromAssemblyContaining<CrearTransaccionValidator>();
        servicios.AddValidatorsFromAssemblyContaining<ActualizarTransaccionValidator>();
        return servicios;
    }
}