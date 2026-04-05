using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

using Sistema.Inventario.Transaccion.Aplicacion.DTOs.Requests;
using Sistema.Inventario.Transaccion.Aplicacion.DTOs.Responses;
using Sistema.Inventario.Transaccion.Dominio.Entidades;
using Sistema.Inventario.Transaccion.Infraestructura.ClientesExternos;
using Sistema.Inventario.Transaccion.Infraestructura.Persistencia;

namespace Sistema.Inventario.Transaccion.Tests.PruebasIntegracion;

/// <summary>
/// Pruebas de integración HTTP para transacciones usando WebApplicationFactory y EF Core InMemory
/// </summary>
public class TransaccionesControllerIntegracionTests : IClassFixture<WebApplicationFactory<Program>>
{
    /// <summary>
    /// WebApplicationFactory configurada para usar InMemory y ambiente de Testing
    /// </summary>
    private readonly WebApplicationFactory<Program> _factory;

    /// <summary>
    /// Constructor que configura WebApplicationFactory para usar InMemory y ambiente de Testing
    /// </summary>
    /// <param name="factory">Instancia de WebApplicationFactory proporcionada por xUnit</param>
    public TransaccionesControllerIntegracionTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(servicios =>
            {
                servicios.RemoveAll<DbContextOptions<TransaccionDbContext>>();
                servicios.RemoveAll<IDbContextOptionsConfiguration<TransaccionDbContext>>();
                servicios.RemoveAll<TransaccionDbContext>();

                servicios.AddDbContext<TransaccionDbContext>(opciones =>
                    opciones.UseInMemoryDatabase("InventarioTransaccionesTestBD"));

                // Mock del cliente API de Productos para pruebas de integración
                servicios.RemoveAll<IProductoApiCliente>();
                Mock<IProductoApiCliente> productoApiClienteMock = new();
                productoApiClienteMock
                    .Setup(cliente => cliente.ObtenerProductoPorIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync((Guid id) => new ProductoExternoResponse { Id = id, Nombre = "Producto Test", Stock = 1000, Precio = 10m });
                productoApiClienteMock
                    .Setup(cliente => cliente.AjustarStockAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()))
                    .ReturnsAsync(true);
                servicios.AddScoped<IProductoApiCliente>(_ => productoApiClienteMock.Object);
            });
        });
    }

    /// <summary>
    /// Reinicia la base InMemory de transacciones para garantizar independencia entre pruebas
    /// </summary>
    private async Task ReiniciarBaseAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        TransaccionDbContext contextoDb = scope.ServiceProvider.GetRequiredService<TransaccionDbContext>();
        await contextoDb.Database.EnsureDeletedAsync();
        await contextoDb.Database.EnsureCreatedAsync();
    }

    /// <summary>
    /// Inserta una transacción de prueba en la base InMemory
    /// </summary>
    /// <param name="tipoTransaccion">Tipo de la transacción (e.g., "Venta")</param>
    /// <param name="cantidad">Cantidad de productos en la transacción</param>
    /// <param name="precioUnitario">Precio unitario de cada producto</param>
    /// <param name="detalle">Detalle de la transacción</param>
    private async Task InsertarTransaccionAsync(string tipoTransaccion, int cantidad, decimal precioUnitario, string detalle)
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        TransaccionDbContext contextoDb = scope.ServiceProvider.GetRequiredService<TransaccionDbContext>();

        await contextoDb.Transacciones.AddAsync(new TransaccionEntidad
        {
            Id = Guid.NewGuid(),
            Fecha = new DateTime(2026, 3, 25, 8, 0, 0),
            TipoTransaccion = tipoTransaccion,
            ProductoId = Guid.NewGuid(),
            Cantidad = cantidad,
            PrecioUnitario = precioUnitario,
            PrecioTotal = cantidad * precioUnitario,
            Detalle = detalle
        });

        await contextoDb.SaveChangesAsync();
    }

    /// <summary>
    /// Valida que GET /api/transacciones retorne 200 Ok con las transacciones existentes
    /// </summary>
    [Fact]
    public async Task ObtenerTransacciones_CuandoExisteData_Retorna200()
    {
        // ARRANGE: Preparar base de datos con una transacción
        await ReiniciarBaseAsync();
        await InsertarTransaccionAsync("Compra", 2, 30m, "Compra inicial");
        using HttpClient cliente = _factory.CreateClient();

        // ACT: Ejecutar consulta HTTP del listado de transacciones
        HttpResponseMessage respuesta = await cliente.GetAsync("/api/transacciones");

        // ASSERT: Verificar código de estado y cantidad de elementos retornados
        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        List<object>? contenido = await respuesta.Content.ReadFromJsonAsync<List<object>>();
        Assert.NotNull(contenido);
        Assert.Single(contenido!);
    }

    /// <summary>
    /// Valida que POST /api/transacciones persista correctamente y retorne 201 Created
    /// </summary>
    [Fact]
    public async Task CrearTransaccion_DebePersistir_Retorna201()
    {
        // ARRANGE: Preparar base de datos limpia y solicitud válida de creación
        await ReiniciarBaseAsync();
        using HttpClient cliente = _factory.CreateClient();

        CrearTransaccionRequest request = new()
        {
            TipoTransaccion = "Compra",
            ProductoId = Guid.NewGuid(),
            Cantidad = 5,
            PrecioUnitario = 11m,
            Detalle = "Compra quincenal"
        };

        // ACT: Ejecutar la creación HTTP de la transacción
        HttpResponseMessage respuesta = await cliente.PostAsJsonAsync("/api/transacciones", request);

        // ASSERT: Verificar código de estado y persistencia en InMemory
        Assert.Equal(HttpStatusCode.Created, respuesta.StatusCode);

        using IServiceScope scope = _factory.Services.CreateScope();
        TransaccionDbContext contextoDb = scope.ServiceProvider.GetRequiredService<TransaccionDbContext>();
        TransaccionEntidad? transaccion = await contextoDb.Transacciones.FirstOrDefaultAsync(transaccion => transaccion.Detalle == "Compra quincenal");

        Assert.NotNull(transaccion);
        Assert.Equal("Compra", transaccion!.TipoTransaccion);
        Assert.Equal(55m, transaccion.PrecioTotal);
    }

    /// <summary>
    /// Valida que GET /api/transacciones/{id} retorne 404 NotFound cuando la transacción no existe
    /// </summary>
    [Fact]
    public async Task ObtenerTransaccionPorId_CuandoNoExiste_Retorna404()
    {
        // ARRANGE: Preparar base de datos limpia y un identificador inexistente
        await ReiniciarBaseAsync();
        using HttpClient cliente = _factory.CreateClient();
        Guid idNoExistente = Guid.NewGuid();

        // ACT: Ejecutar consulta HTTP por identificador inexistente
        HttpResponseMessage respuesta = await cliente.GetAsync($"/api/transacciones/{idNoExistente}");

        // ASSERT: Verificar respuesta HTTP 404 NotFound
        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
    }

    /// <summary>
    /// Valida que PUT /api/transacciones/{id} actualice datos y retorne 200 Ok
    /// </summary>
    [Fact]
    public async Task ActualizarTransaccion_CuandoExiste_Retorna200()
    {
        // ARRANGE: Crear transacción base y preparar solicitud de actualización
        await ReiniciarBaseAsync();
        using HttpClient cliente = _factory.CreateClient();

        CrearTransaccionRequest transaccionRequest = new()
        {
            TipoTransaccion = "Compra",
            ProductoId = Guid.NewGuid(),
            Cantidad = 4,
            PrecioUnitario = 10m,
            Detalle = "Transacción base"
        };

        HttpResponseMessage responseCrear = await cliente.PostAsJsonAsync("/api/transacciones", transaccionRequest);
        TransaccionResponse? transaccion = await responseCrear.Content.ReadFromJsonAsync<TransaccionResponse>();

        Assert.Equal(HttpStatusCode.Created, responseCrear.StatusCode);
        Assert.NotNull(transaccion);

        ActualizarTransaccionRequest request = new()
        {
            TipoTransaccion = "Venta",
            ProductoId = transaccion!.ProductoId,
            Cantidad = 3,
            PrecioUnitario = 15m,
            Detalle = "Transacción actualizada"
        };

        // ACT: Ejecutar actualización HTTP de la transacción
        HttpResponseMessage response = await cliente.PutAsJsonAsync($"/api/transacciones/{transaccion.Id}", request);

        // ASSERT: Verificar respuesta correcta y persistencia de cambios
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using IServiceScope scope = _factory.Services.CreateScope();
        TransaccionDbContext contextoDb = scope.ServiceProvider.GetRequiredService<TransaccionDbContext>();
        TransaccionEntidad? transaccionActualizada = await contextoDb.Transacciones.FirstOrDefaultAsync(transaccion => transaccion.Id == transaccion.Id);

        Assert.NotNull(transaccionActualizada);
        Assert.Equal("Venta", transaccionActualizada!.TipoTransaccion);
        Assert.Equal(45m, transaccionActualizada.PrecioTotal);
        Assert.Equal("Transacción actualizada", transaccionActualizada.Detalle);
    }

    /// <summary>
    /// Valida que DELETE /api/transacciones/{id} elimine el registro y retorne 204 NoContent
    /// </summary>
    [Fact]
    public async Task EliminarTransaccion_CuandoExiste_Retorna204()
    {
        // ARRANGE: Crear una transacción existente que será eliminada
        await ReiniciarBaseAsync();
        using HttpClient cliente = _factory.CreateClient();

        CrearTransaccionRequest transaccionRequest = new()
        {
            TipoTransaccion = "Venta",
            ProductoId = Guid.NewGuid(),
            Cantidad = 2,
            PrecioUnitario = 22m,
            Detalle = "Transacción eliminable"
        };

        HttpResponseMessage responseCrear = await cliente.PostAsJsonAsync("/api/transacciones", transaccionRequest);
        TransaccionResponse? transaccion = await responseCrear.Content.ReadFromJsonAsync<TransaccionResponse>();

        Assert.Equal(HttpStatusCode.Created, responseCrear.StatusCode);
        Assert.NotNull(transaccion);

        // ACT: Ejecutar eliminación HTTP de la transacción creada
        HttpResponseMessage response = await cliente.DeleteAsync($"/api/transacciones/{transaccion!.Id}");

        // ASSERT: Verificar respuesta 204 y ausencia del registro en la base
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using IServiceScope scope = _factory.Services.CreateScope();
        TransaccionDbContext contextoDb = scope.ServiceProvider.GetRequiredService<TransaccionDbContext>();
        TransaccionEntidad? transaccionEliminada = await contextoDb.Transacciones.FirstOrDefaultAsync(transaccion => transaccion.Id == transaccion.Id);
        Assert.Null(transaccionEliminada);
    }

    /// <summary>
    /// Valida que POST /api/transacciones retorne 400 BadRequest cuando el request es inválido
    /// </summary>
    [Fact]
    public async Task CrearTransaccion_CuandoRequestEsInvalido_Retorna400()
    {
        // ARRANGE: Preparar solicitud con datos inválidos (campos vacíos)
        await ReiniciarBaseAsync();
        using HttpClient cliente = _factory.CreateClient();

        CrearTransaccionRequest requestInvalido = new()
        {
            TipoTransaccion = "",
            ProductoId = Guid.Empty,
            Cantidad = 0,
            PrecioUnitario = -5m,
            Detalle = ""
        };

        // ACT: Ejecutar creación con datos inválidos
        HttpResponseMessage respuesta = await cliente.PostAsJsonAsync("/api/transacciones", requestInvalido);

        // ASSERT: Verificar que el servidor retorne 400 BadRequest
        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
    }

    /// <summary>
    /// Valida que GET /api/transacciones/{id} retorne 200 Ok con los datos correctos cuando la transacción existe
    /// </summary>
    [Fact]
    public async Task ObtenerTransaccionPorId_CuandoExiste_Retorna200()
    {
        // ARRANGE: Crear una transacción y capturar su identificador
        await ReiniciarBaseAsync();
        using HttpClient cliente = _factory.CreateClient();
        await InsertarTransaccionAsync("Compra", 4, 15m, "Compra para búsqueda por id");

        using IServiceScope alcanceConfig = _factory.Services.CreateScope();
        TransaccionDbContext contextoDbConfig = alcanceConfig.ServiceProvider.GetRequiredService<TransaccionDbContext>();
        TransaccionEntidad transaccionInsertada = await contextoDbConfig.Transacciones.FirstAsync();

        // ACT: Consultar la transacción por su identificador
        HttpResponseMessage respuesta = await cliente.GetAsync($"/api/transacciones/{transaccionInsertada.Id}");

        // ASSERT: Verificar código 200 y datos correctos en el cuerpo
        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        TransaccionResponse? contenido = await respuesta.Content.ReadFromJsonAsync<TransaccionResponse>();
        Assert.NotNull(contenido);
        Assert.Equal(transaccionInsertada.Id, contenido!.Id);
        Assert.Equal("Compra", contenido.TipoTransaccion);
        Assert.Equal(60m, contenido.PrecioTotal);
        Assert.Equal("Compra para búsqueda por id", contenido.Detalle);
    }

    /// <summary>
    /// Valida que PUT /api/transacciones/{id} retorne 404 NotFound cuando la transacción no existe
    /// </summary>
    [Fact]
    public async Task ActualizarTransaccion_CuandoNoExiste_Retorna404()
    {
        // ARRANGE: Preparar base limpia y un identificador inexistente
        await ReiniciarBaseAsync();
        using HttpClient cliente = _factory.CreateClient();
        Guid idNoExistente = Guid.NewGuid();

        ActualizarTransaccionRequest request = new()
        {
            TipoTransaccion = "Venta",
            ProductoId = Guid.NewGuid(),
            Cantidad = 2,
            PrecioUnitario = 30m,
            Detalle = "Actualización fallida"
        };

        // ACT: Ejecutar actualización HTTP con identificador inexistente
        HttpResponseMessage respuesta = await cliente.PutAsJsonAsync($"/api/transacciones/{idNoExistente}", request);

        // ASSERT: Verificar respuesta 404 NotFound
        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
    }

    /// <summary>
    /// Valida que DELETE /api/transacciones/{id} retorne 404 NotFound cuando la transacción no existe
    /// </summary>
    [Fact]
    public async Task EliminarTransaccion_CuandoNoExiste_Retorna404()
    {
        // ARRANGE: Preparar base limpia y un identificador inexistente
        await ReiniciarBaseAsync();
        using HttpClient cliente = _factory.CreateClient();
        Guid idNoExistente = Guid.NewGuid();

        // ACT: Ejecutar eliminación HTTP con identificador inexistente
        HttpResponseMessage respuesta = await cliente.DeleteAsync($"/api/transacciones/{idNoExistente}");

        // ASSERT: Verificar respuesta 404 NotFound
        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
    }
}
