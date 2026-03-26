using Microsoft.Extensions.Logging;
using Moq;
using Sistema.Inventario.Producto.Aplicacion.DTOs.Requests;
using Sistema.Inventario.Producto.Aplicacion.Servicios;
using Sistema.Inventario.Producto.Dominio.Entidades;
using Sistema.Inventario.Producto.Infraestructura.Repositorios;

namespace Sistema.Inventario.Producto.Tests.PruebasUnitarias;

/// <summary>
/// Pruebas unitarias para ProductoServicio usando Moq para el repositorio
/// </summary>
public class ProductoServicioTests
{
    /// <summary>
    /// Mock para el repositorio de productos
    /// </summary>
    private readonly Mock<IProductoRepositorio> _repositorioProductoMock = new();

    /// <summary>
    /// Mock para el servicio de logging
    /// </summary>
    private readonly Mock<ILogger<ProductoServicio>> _loggerMock = new();

    /// <summary>
    /// Crea una instancia de ProductoServicio con los mocks configurados
    /// </summary>
    /// <returns>Instancia de ProductoServicio</returns>
    private ProductoServicio CrearServicio()
    {
        return new ProductoServicio(_repositorioProductoMock.Object, _loggerMock.Object);
    }

    /// <summary>
    /// Valida que el servicio mapee correctamente los productos obtenidos del repositorio
    /// </summary>
    [Fact]
    public async Task ObtenerProductosAsync_CuandoExistenRegistros_DebeMapearLosProductos()
    {
        // ARRANGE: Preparar datos de prueba y configurar mock
        List<ProductoEntidad> productosEntidad =
        [
            new ProductoEntidad
            {
                Id = Guid.NewGuid(),
                Nombre = "Mouse",
                Descripcion = "Mouse inalambrico",
                Categoria = "Perifericos",
                ImagenUrl = "https://sistemainventario.com/productos/mouse.png",
                Precio = 25.99m,
                Stock = 8
            },
            new ProductoEntidad
            {
                Id = Guid.NewGuid(),
                Nombre = "Teclado",
                Descripcion = "Teclado mecanico",
                Categoria = "Perifericos",
                ImagenUrl = "https://sistemainventario.com/productos/teclado.png",
                Precio = 59.99m,
                Stock = 5
            }
        ];

        _repositorioProductoMock
            .Setup(repositorio => repositorio.ObtenerProductosAsync())
            .ReturnsAsync(productosEntidad);

        ProductoServicio servicio = CrearServicio();

        // ACT: Obtener productos del servicio
        List<Sistema.Inventario.Producto.Aplicacion.DTOs.Responses.ProductoResponse> resultado = await servicio.ObtenerProductosAsync();

        // ASSERT: Verificar mapeo correcto y cantidad de registros
        Assert.Equal(2, resultado.Count);
        Assert.Collection(resultado,
            producto =>
            {
                Assert.Equal(productosEntidad[0].Id, producto.Id);
                Assert.Equal("Mouse", producto.Nombre);
            },
            producto =>
            {
                Assert.Equal(productosEntidad[1].Id, producto.Id);
                Assert.Equal("Teclado", producto.Nombre);
            });
    }

    /// <summary>
    /// Valida que el servicio retorne null cuando el producto no existe
    /// </summary>
    [Fact]
    public async Task ObtenerProductoPorIdAsync_CuandoNoExiste_DebeRetornarNull()
    {
        // ARRANGE: Preparar ID de prueba y mock que retorna null
        Guid idProducto = Guid.NewGuid();

        _repositorioProductoMock
            .Setup(repositorio => repositorio.ObtenerProductoPorIdAsync(idProducto))
            .ReturnsAsync((ProductoEntidad?)null);

        ProductoServicio servicio = CrearServicio();

        // ACT: Buscar producto que no existe
        Sistema.Inventario.Producto.Aplicacion.DTOs.Responses.ProductoResponse? resultado = await servicio.ObtenerProductoPorIdAsync(idProducto);

        // ASSERT: Verificar que el resultado es null
        Assert.Null(resultado);
    }

    /// <summary>
    /// Valida que el servicio persista correctamente el nuevo producto
    /// </summary>
    [Fact]
    public async Task CrearProductoAsync_CuandoElRequestEsValido_DebePersistirYRetornarElProducto()
    {
        // ARRANGE: Preparar request de creación y mock del repositorio
        CrearProductoRequest request = new()
        {
            Nombre = "Impresora",
            Descripcion = "Impresora laser",
            Categoria = "Oficina",
            ImagenUrl = "https://sistemainventario.com/productos/impresora.png",
            Precio = 310.75m,
            Stock = 4
        };
        ProductoEntidad? productoCreado = null;

        _repositorioProductoMock
            .Setup(repositorio => repositorio.CrearProductoAsync(It.IsAny<ProductoEntidad>()))
            .Callback<ProductoEntidad>(producto => productoCreado = producto)
            .Returns(Task.CompletedTask);

        ProductoServicio servicio = CrearServicio();

        // ACT: Crear producto a través del servicio
        Sistema.Inventario.Producto.Aplicacion.DTOs.Responses.ProductoResponse resultado = await servicio.CrearProductoAsync(request);

        // ASSERT: Verificar que se persistió correctamente y se mapea bien
        Assert.NotNull(productoCreado);
        Assert.NotEqual(Guid.Empty, productoCreado!.Id);
        Assert.Equal(request.Nombre, productoCreado.Nombre);
        Assert.Equal(request.Descripcion, productoCreado.Descripcion);
        Assert.Equal(request.Categoria, productoCreado.Categoria);
        Assert.Equal(request.ImagenUrl, productoCreado.ImagenUrl);
        Assert.Equal(request.Precio, productoCreado.Precio);
        Assert.Equal(request.Stock, productoCreado.Stock);

        Assert.Equal(productoCreado.Id, resultado.Id);
        Assert.Equal(request.Nombre, resultado.Nombre);

        _repositorioProductoMock.Verify(repositorio => repositorio.CrearProductoAsync(It.IsAny<ProductoEntidad>()), Times.Once);
    }

    /// <summary>
    /// Valida que el servicio actualice correctamente los datos del producto
    /// </summary>
    [Fact]
    public async Task ActualizarProductoAsync_CuandoElProductoExiste_DebeRetornarElProductoActualizado()
    {
        // ARRANGE: Preparar ID de producto, request de actualización y mock del repositorio
        Guid idProducto = Guid.NewGuid();
        ActualizarProductoRequest request = new()
        {
            Nombre = "Tablet",
            Descripcion = "Tablet 10 pulgadas",
            Categoria = "Tecnologia",
            ImagenUrl = "https://sistemainventario.com/productos/tablet.png",
            Precio = 220.00m,
            Stock = 7
        };

        _repositorioProductoMock
            .Setup(repositorio => repositorio.ActualizarProductoAsync(idProducto, It.IsAny<ProductoEntidad>()))
            .ReturnsAsync((Guid productoId, ProductoEntidad producto) =>
            {
                producto.Id = productoId;
                return producto;
            });

        ProductoServicio servicio = CrearServicio();

        // ACT: Actualizar producto a través del servicio
        Sistema.Inventario.Producto.Aplicacion.DTOs.Responses.ProductoResponse? resultado = await servicio.ActualizarProductoAsync(idProducto, request);

        // ASSERT: Verificar que los datos se actualizaron correctamente
        Assert.NotNull(resultado);
        Assert.Equal(idProducto, resultado!.Id);
        Assert.Equal(request.Nombre, resultado.Nombre);
        Assert.Equal(request.Precio, resultado.Precio);
        Assert.Equal(request.Stock, resultado.Stock);
    }

    /// <summary>
    /// Valida que el servicio retorne false cuando el producto a eliminar no existe
    /// </summary>
    [Fact]
    public async Task EliminarProductoAsync_CuandoElProductoNoExiste_RetornaFalso()
    {
        // ARRANGE: Preparar ID de prueba y mock que retorna false
        Guid idProducto = Guid.NewGuid();

        _repositorioProductoMock
            .Setup(repositorio => repositorio.EliminarProductoAsync(idProducto))
            .ReturnsAsync(false);

        ProductoServicio servicio = CrearServicio();

        // ACT: Intentar eliminar producto que no existe
        bool resultado = await servicio.EliminarProductoAsync(idProducto);

        // ASSERT: Verificar que retorna false
        Assert.False(resultado);
    }
}