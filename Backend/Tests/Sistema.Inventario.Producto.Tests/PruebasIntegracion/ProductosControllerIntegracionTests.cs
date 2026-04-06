using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

using Sistema.Inventario.Producto.Aplicacion.DTOs.Requests;
using Sistema.Inventario.Producto.Aplicacion.DTOs.Responses;
using Sistema.Inventario.Producto.Dominio.Entidades;
using Sistema.Inventario.Producto.Infraestructura.Persistencia;

using Sistema.Inventario.Storage.DTOs.Requests;
using Sistema.Inventario.Storage.DTOs.Responses;
using Sistema.Inventario.Storage.Servicios;

namespace Sistema.Inventario.Producto.Tests.PruebasIntegracion;

/// <summary>
/// Pruebas de integración HTTP usando WebApplicationFactory y EF Core InMemory
/// Valida el pipeline completo del microservicio de Productos
/// </summary>
public class ProductosControllerIntegracionTests : IClassFixture<WebApplicationFactory<Program>>
{
    /// <summary>
    /// WebApplicationFactory configurada para usar EF Core InMemory y ambiente de Testing
    /// </summary>
    private readonly WebApplicationFactory<Program> _factory;

    /// <summary>
    /// Mock del servicio de almacenamiento para evitar I/O real en pruebas de integración
    /// </summary>
    private readonly Mock<IAlmacenamientoServicio> _almacenamientoServicioMock = new();

    /// <summary>
    /// Constructor que configura WebApplicationFactory para usar EF Core InMemory y ambiente de Testing
    /// </summary>
    /// <param name="factory">WebApplicationFactory configurada para el microservicio de Productos</param>
    public ProductosControllerIntegracionTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(servicios =>
            {
                servicios.RemoveAll<DbContextOptions<ProductoDbContext>>();
                servicios.RemoveAll<IDbContextOptionsConfiguration<ProductoDbContext>>();
                servicios.RemoveAll<ProductoDbContext>();

                servicios.AddDbContext<ProductoDbContext>(opciones =>
                    opciones.UseInMemoryDatabase("InventarioProductosTestBD"));

                servicios.RemoveAll<IAlmacenamientoServicio>();
                servicios.AddSingleton(_almacenamientoServicioMock.Object);
            });
        });
    }

    /// <summary>
    /// Reinicia la base de datos en memoria eliminando y recreando el esquema
    /// </summary>
    private async Task ReiniciarBaseAsync()
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        ProductoDbContext contextoDb = scope.ServiceProvider.GetRequiredService<ProductoDbContext>();
        await contextoDb.Database.EnsureDeletedAsync();
        await contextoDb.Database.EnsureCreatedAsync();
    }

    /// <summary>
    /// Inserta datos de prueba en la base de datos en memoria
    /// </summary>
    /// <param name="nombre">Nombre del producto a insertar</param>
    /// <param name="precio">Precio del producto a insertar</param>
    /// <param name="stock">Stock del producto a insertar</param>
    private async Task InsertarProductoAsync(string nombre, decimal precio, int stock)
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        ProductoDbContext contextoDb = scope.ServiceProvider.GetRequiredService<ProductoDbContext>();

        await contextoDb.Productos.AddAsync(new ProductoEntidad
        {
            Id = Guid.NewGuid(),
            Nombre = nombre,
            Descripcion = "Producto semilla",
            Categoria = "General",
            ImagenUrl = "https://sistemainventario.com/productos/semilla.png",
            Precio = precio,
            Stock = stock
        });

        await contextoDb.SaveChangesAsync();
    }

    /// <summary>
    /// Valida que GET /api/productos retorne 200 Ok con los productos existentes
    /// </summary>
    [Fact]
    public async Task ObtenerProductos_CuandoExisteData_Retorna200()
    {
        // ARRANGE: Preparar base de datos con datos de prueba
        await ReiniciarBaseAsync();
        await InsertarProductoAsync("Mouse Gamer", 35m, 7);

        using HttpClient cliente = _factory.CreateClient();

        // ACT: Realizar petición GET a /api/productos
        HttpResponseMessage respuesta = await cliente.GetAsync("/api/productos");

        // ASSERT: Verificar status 200 y que hay un producto en la respuesta
        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        List<object>? contenido = await respuesta.Content.ReadFromJsonAsync<List<object>>();
        Assert.NotNull(contenido);
        Assert.Single(contenido!);
    }

    /// <summary>
    /// Valida que POST /api/productos persista correctamente en EF InMemory y retorne 201 Created
    /// </summary>
    [Fact]
    public async Task CrearProducto_DebePersistirEnEfInMemory_Retorna201()
    {
        // ARRANGE: Preparar base de datos limpia y un nuevo request
        await ReiniciarBaseAsync();
        using HttpClient cliente = _factory.CreateClient();

        CrearProductoRequest productoRequest = new()
        {
            Nombre = "Audifonos",
            Descripcion = "Audifonos bluetooth",
            Categoria = "Audio",
            ImagenUrl = "https://sistemainventario.com/productos/audifonos.png",
            Precio = 80,
            Stock = 15
        };

        // ACT: Realizar POST para crear producto
        HttpResponseMessage respuesta = await cliente.PostAsJsonAsync("/api/productos", productoRequest);

        // ASSERT: Verificar status 201 y que se persistió en base de datos
        Assert.Equal(HttpStatusCode.Created, respuesta.StatusCode);

        using IServiceScope scope = _factory.Services.CreateScope();
        ProductoDbContext contextoDb = scope.ServiceProvider.GetRequiredService<ProductoDbContext>();
        ProductoEntidad? productoCreado = await contextoDb.Productos.FirstOrDefaultAsync(producto => producto.Nombre == "Audifonos");

        Assert.NotNull(productoCreado);
        Assert.Equal(80, productoCreado!.Precio);
        Assert.Equal(15, productoCreado.Stock);
    }

    /// <summary>
    /// Valida que GET /api/productos/{id} retorne 404 NotFound cuando el producto no existe
    /// </summary>
    [Fact]
    public async Task ObtenerProductoPorId_CuandoNoExiste_Retorna404()
    {
        // ARRANGE: Preparar base de datos limpia y generar un ID que no existe
        await ReiniciarBaseAsync();
        using HttpClient cliente = _factory.CreateClient();

        Guid idNoExistente = Guid.NewGuid();

        // ACT: Realizar GET para buscar producto inexistente
        HttpResponseMessage respuesta = await cliente.GetAsync($"/api/productos/{idNoExistente}");

        // ASSERT: Verificar que retorna 404 NotFound
        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
    }

    /// <summary>
    /// Valida que PUT /api/productos/{id} actualice correctamente los datos en EF InMemory y retorne 200 OK
    /// </summary>
    [Fact]
    public async Task ActualizarProducto_CuandoExiste_Retorna200()
    {
        // ARRANGE: Preparar base de datos limpia, crear un producto y preparar datos de actualización
        await ReiniciarBaseAsync();
        using HttpClient cliente = _factory.CreateClient();

        // Crear un producto existente
        CrearProductoRequest productoRequest = new()
        {
            Nombre = "Impresora Base",
            Descripcion = "Impresora inicial",
            Categoria = "Oficina",
            ImagenUrl = "https://sistemainventario.com/productos/impresora-base.png",
            Precio = 150,
            Stock = 4
        };

        HttpResponseMessage respuesta = await cliente.PostAsJsonAsync("/api/productos", productoRequest);
        ProductoResponse? productoCreado = await respuesta.Content.ReadFromJsonAsync<ProductoResponse>();

        Assert.Equal(HttpStatusCode.Created, respuesta.StatusCode);
        Assert.NotNull(productoCreado);

        // Preparar datos de actualización
        ActualizarProductoRequest productoActualizarRequest = new()
        {
            Nombre = "Impresora Pro",
            Descripcion = "Impresora actualizada",
            Categoria = "Oficina",
            ImagenUrl = "https://sistemainventario.com/productos/impresora-pro.png",
            Precio = 180,
            Stock = 6
        };

        // ACT: Realizar PUT para actualizar producto
        HttpResponseMessage response = await cliente.PutAsJsonAsync($"/api/productos/{productoCreado!.Id}", productoActualizarRequest);

        // ASSERT: Verificar status 200 y que se persistieron los cambios
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using IServiceScope scope = _factory.Services.CreateScope();
        ProductoDbContext contextoDb = scope.ServiceProvider.GetRequiredService<ProductoDbContext>();
        ProductoEntidad? productoActualizado = await contextoDb.Productos.FirstOrDefaultAsync(producto => producto.Id == productoCreado.Id);

        Assert.NotNull(productoActualizado);
        Assert.Equal("Impresora Pro", productoActualizado!.Nombre);
        Assert.Equal(180, productoActualizado.Precio);
        Assert.Equal(6, productoActualizado.Stock);
    }

    /// <summary>
    /// Valida que DELETE /api/productos/{id} elimine correctamente el registro en EF InMemory y retorne 204 NoContent
    /// </summary>
    [Fact]
    public async Task EliminarProducto_CuandoExiste_Retorna204()
    {
        // ARRANGE: Preparar base de datos limpia y crear un producto para eliminar
        await ReiniciarBaseAsync();
        using HttpClient cliente = _factory.CreateClient();

        CrearProductoRequest productoRequest = new()
        {
            Nombre = "Producto Eliminable",
            Descripcion = "Se elimina en la prueba",
            Categoria = "General",
            ImagenUrl = "https://sistemainventario.com/productos/eliminable.png",
            Precio = 10,
            Stock = 1
        };

        HttpResponseMessage respuesta = await cliente.PostAsJsonAsync("/api/productos", productoRequest);
        ProductoResponse? productoResonse = await respuesta.Content.ReadFromJsonAsync<ProductoResponse>();

        Assert.Equal(HttpStatusCode.Created, respuesta.StatusCode);
        Assert.NotNull(productoResonse);

        // ACT: Realizar DELETE para eliminar producto
        HttpResponseMessage response = await cliente.DeleteAsync($"/api/productos/{productoResonse!.Id}");

        // ASSERT: Verificar status 204 y que se eliminó de base de datos
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using IServiceScope alcance = _factory.Services.CreateScope();
        ProductoDbContext contextoDb = alcance.ServiceProvider.GetRequiredService<ProductoDbContext>();
        ProductoEntidad? productoEliminado = await contextoDb.Productos.FirstOrDefaultAsync(producto => producto.Id == productoResonse.Id);
        Assert.Null(productoEliminado);
    }

    /// <summary>
    /// Valida que PATCH /api/productos/{id}/stock ajuste correctamente el stock y retorne 200 OK
    /// </summary>
    [Fact]
    public async Task AjustarStock_CuandoCompraValidaActualizaStock_Retorna200()
    {
        // ARRANGE: Preparar base limpia y crear un producto inicial con stock conocido
        await ReiniciarBaseAsync();
        using HttpClient cliente = _factory.CreateClient();

        CrearProductoRequest productoRequest = new()
        {
            Nombre = "Producto Stock",
            Descripcion = "Producto para prueba de patch",
            Categoria = "General",
            ImagenUrl = "https://sistemainventario.com/productos/stock.png",
            Precio = 50,
            Stock = 10
        };

        HttpResponseMessage respuestaCrear = await cliente.PostAsJsonAsync("/api/productos", productoRequest);
        ProductoResponse? productoCreado = await respuestaCrear.Content.ReadFromJsonAsync<ProductoResponse>();

        Assert.Equal(HttpStatusCode.Created, respuestaCrear.StatusCode);
        Assert.NotNull(productoCreado);

        AjustarStockRequest request = new()
        {
            Cantidad = 3,
            TipoOperacion = "Compra"
        };

        // ACT: Ajustar stock con operación de compra
        HttpResponseMessage respuestaPatch = await cliente.PatchAsJsonAsync($"/api/productos/{productoCreado!.Id}/stock", request);

        // ASSERT: Verificar respuesta 200 y stock incrementado en base de datos
        Assert.Equal(HttpStatusCode.OK, respuestaPatch.StatusCode);

        using IServiceScope scope = _factory.Services.CreateScope();
        ProductoDbContext contextoDb = scope.ServiceProvider.GetRequiredService<ProductoDbContext>();
        ProductoEntidad? productoActualizado = await contextoDb.Productos.FirstOrDefaultAsync(producto => producto.Id == productoCreado.Id);

        Assert.NotNull(productoActualizado);
        Assert.Equal(13, productoActualizado!.Stock);
    }

    /// <summary>
    /// Valida que POST /api/productos/imagenes con archivo válido retorne 200 OK con la URL pública
    /// </summary>
    [Fact]
    public async Task SubirImagen_CuandoArchivoValidoConImagenUrl_Retorna200()
    {
        // ARRANGE: Configurar mock del servicio de almacenamiento con respuesta exitosa
        _almacenamientoServicioMock.Reset();
        string urlEsperada = "http://localhost:5261/imagenes/test-uuid.jpg";
        _almacenamientoServicioMock
            .Setup(s => s.GuardarArchivoAsync(It.IsAny<ArchivoImagenRequest>()))
            .ReturnsAsync(new ArchivoImagenResponse
            {
                NombreArchivo = "test-uuid.jpg",
                UrlImagen = urlEsperada,
                TipoContenido = "image/jpeg",
                TamanioBytes = 1024
            });

        using HttpClient cliente = _factory.CreateClient();

        // cabecera JPEG mínima
        byte[] contenidoArchivo = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 };
        using ByteArrayContent bytesArchivo = new(contenidoArchivo);
        bytesArchivo.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

        using MultipartFormDataContent formulario = new();
        formulario.Add(bytesArchivo, "archivoImagen", "producto.jpg");

        // ACT: Realizar POST multipart al endpoint de imágenes
        HttpResponseMessage respuesta = await cliente.PostAsync("/api/productos/imagenes", formulario);

        // ASSERT: Verificar status 200 y que la URL retornada es la del servicio
        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        SubirImagenResponse? contenido = await respuesta.Content.ReadFromJsonAsync<SubirImagenResponse>();
        Assert.NotNull(contenido);
        Assert.Equal(urlEsperada, contenido!.ImagenUrl);
    }

    /// <summary>
    /// Valida que POST /api/productos/imagenes retorne 400 BadRequest cuando el servicio rechaza el archivo
    /// </summary>
    [Fact]
    public async Task SubirImagen_CuandoServicioRechazaArchivo_Retorna400()
    {
        // ARRANGE: Configurar mock del servicio para lanzar ArgumentException
        _almacenamientoServicioMock.Reset();
        _almacenamientoServicioMock
            .Setup(s => s.GuardarArchivoAsync(It.IsAny<ArchivoImagenRequest>()))
            .ThrowsAsync(new ArgumentException("La extensión del archivo no está permitida. Use jpg, jpeg, png o webp."));

        using HttpClient cliente = _factory.CreateClient();

        // cabecera EXE
        byte[] contenidoArchivo = new byte[] { 0x4D, 0x5A };
        using ByteArrayContent bytesArchivo = new(contenidoArchivo);
        bytesArchivo.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

        using MultipartFormDataContent formulario = new();
        formulario.Add(bytesArchivo, "archivoImagen", "malware.exe");

        // ACT: Realizar POST multipart con archivo inválido
        HttpResponseMessage respuesta = await cliente.PostAsync("/api/productos/imagenes", formulario);

        // ASSERT: Verificar que retorna 400 BadRequest
        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
    }
}