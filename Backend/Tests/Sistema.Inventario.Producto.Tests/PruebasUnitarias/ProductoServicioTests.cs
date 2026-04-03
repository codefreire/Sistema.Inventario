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

    /// <summary>
    /// Valida que el servicio retorne el producto correctamente cuando existe
    /// </summary>
    [Fact]
    public async Task ObtenerProductoPorIdAsync_CuandoExiste_DebeRetornarProducto()
    {
        // ARRANGE: Preparar entidad de producto y mock que la devuelve
        Guid idProducto = Guid.NewGuid();
        ProductoEntidad entidad = new()
        {
            Id = idProducto,
            Nombre = "Webcam",
            Descripcion = "Webcam Full HD",
            Categoria = "Perifericos",
            ImagenUrl = "https://sistemainventario.com/productos/webcam.png",
            Precio = 75m,
            Stock = 12
        };

        _repositorioProductoMock
            .Setup(repositorio => repositorio.ObtenerProductoPorIdAsync(idProducto))
            .ReturnsAsync(entidad);

        ProductoServicio servicio = CrearServicio();

        // ACT: Obtener el producto por su Id
        Sistema.Inventario.Producto.Aplicacion.DTOs.Responses.ProductoResponse? resultado = await servicio.ObtenerProductoPorIdAsync(idProducto);

        // ASSERT: Verificar que se mapean correctamente todos los campos
        Assert.NotNull(resultado);
        Assert.Equal(idProducto, resultado!.Id);
        Assert.Equal("Webcam", resultado.Nombre);
        Assert.Equal(75m, resultado.Precio);
        Assert.Equal(12, resultado.Stock);
    }

    /// <summary>
    /// Valida que el servicio retorne null cuando el producto a actualizar no existe
    /// </summary>
    [Fact]
    public async Task ActualizarProductoAsync_CuandoNoExiste_DebeRetornarNull()
    {
        // ARRANGE: Configurar mock del repositorio para retornar null
        Guid idProducto = Guid.NewGuid();
        ActualizarProductoRequest request = new()
        {
            Nombre = "Monitor 24",
            Descripcion = "Monitor LED",
            Categoria = "Tecnologia",
            ImagenUrl = "https://sistemainventario.com/productos/monitor.png",
            Precio = 300m,
            Stock = 5
        };

        _repositorioProductoMock
            .Setup(repositorio => repositorio.ActualizarProductoAsync(idProducto, It.IsAny<ProductoEntidad>()))
            .ReturnsAsync((ProductoEntidad?)null);

        ProductoServicio servicio = CrearServicio();

        // ACT: Intentar actualizar un producto que no existe
        Sistema.Inventario.Producto.Aplicacion.DTOs.Responses.ProductoResponse? resultado = await servicio.ActualizarProductoAsync(idProducto, request);

        // ASSERT: Verificar que retorna null
        Assert.Null(resultado);
    }

    /// <summary>
    /// Valida que el servicio retorne true cuando el producto es eliminado correctamente
    /// </summary>
    [Fact]
    public async Task EliminarProductoAsync_CuandoExiste_DebeRetornarTrue()
    {
        // ARRANGE: Configurar mock del repositorio para confirmar eliminacion exitosa
        Guid idProducto = Guid.NewGuid();

        _repositorioProductoMock
            .Setup(repositorio => repositorio.EliminarProductoAsync(idProducto))
            .ReturnsAsync(true);

        ProductoServicio servicio = CrearServicio();

        // ACT: Eliminar el producto existente
        bool resultado = await servicio.EliminarProductoAsync(idProducto);

        // ASSERT: Verificar que retorna true y se invoco exactamente una vez
        Assert.True(resultado);
        _repositorioProductoMock.Verify(repositorio => repositorio.EliminarProductoAsync(idProducto), Times.Once);
    }

    /// <summary>
    /// Valida que AjustarStockAsync incremente el stock correctamente en una Compra
    /// </summary>
    [Fact]
    public async Task AjustarStockAsync_CuandoCompra_DebeIncrementarElStock()
    {
        // ARRANGE: Producto con stock 10, compra de 5 unidades, nuevo stock esperado 15
        Guid idProducto = Guid.NewGuid();
        ProductoEntidad entidadExistente = new()
        {
            Id = idProducto,
            Nombre = "Router",
            Descripcion = "Router WiFi 6",
            Categoria = "Redes",
            ImagenUrl = "https://sistemainventario.com/productos/router.png",
            Precio = 120m,
            Stock = 10
        };

        _repositorioProductoMock
            .Setup(repositorio => repositorio.ObtenerProductoPorIdAsync(idProducto))
            .ReturnsAsync(entidadExistente);

        _repositorioProductoMock
            .Setup(repositorio => repositorio.AjustarStockAsync(idProducto, 15))
            .ReturnsAsync((Guid id, int nuevoStock) =>
            {
                return new ProductoEntidad
                {
                    Id = id,
                    Nombre = entidadExistente.Nombre,
                    Descripcion = entidadExistente.Descripcion,
                    Categoria = entidadExistente.Categoria,
                    ImagenUrl = entidadExistente.ImagenUrl,
                    Precio = entidadExistente.Precio,
                    Stock = nuevoStock
                };
            });

        ProductoServicio servicio = CrearServicio();

        // ACT: Ajustar stock con tipo Compra
        Sistema.Inventario.Producto.Aplicacion.DTOs.Responses.ProductoResponse? resultado = await servicio.AjustarStockAsync(idProducto, 5, "Compra");

        // ASSERT: Verificar que el stock se incremento correctamente
        Assert.NotNull(resultado);
        Assert.Equal(15, resultado!.Stock);
    }

    /// <summary>
    /// Valida que AjustarStockAsync decremente el stock correctamente en una Venta
    /// </summary>
    [Fact]
    public async Task AjustarStockAsync_CuandoVenta_DebeDecrementarElStock()
    {
        // ARRANGE: Producto con stock 10, venta de 3 unidades, nuevo stock esperado 7
        Guid idProducto = Guid.NewGuid();
        ProductoEntidad entidadExistente = new()
        {
            Id = idProducto,
            Nombre = "Switch",
            Descripcion = "Switch de red 8 puertos",
            Categoria = "Redes",
            ImagenUrl = "https://sistemainventario.com/productos/switch.png",
            Precio = 80m,
            Stock = 10
        };

        _repositorioProductoMock
            .Setup(repositorio => repositorio.ObtenerProductoPorIdAsync(idProducto))
            .ReturnsAsync(entidadExistente);

        _repositorioProductoMock
            .Setup(repositorio => repositorio.AjustarStockAsync(idProducto, 7))
            .ReturnsAsync((Guid id, int nuevoStock) =>
            {
                return new ProductoEntidad
                {
                    Id = id,
                    Nombre = entidadExistente.Nombre,
                    Descripcion = entidadExistente.Descripcion,
                    Categoria = entidadExistente.Categoria,
                    ImagenUrl = entidadExistente.ImagenUrl,
                    Precio = entidadExistente.Precio,
                    Stock = nuevoStock
                };
            });

        ProductoServicio servicio = CrearServicio();

        // ACT: Ajustar stock con tipo Venta
        Sistema.Inventario.Producto.Aplicacion.DTOs.Responses.ProductoResponse? resultado = await servicio.AjustarStockAsync(idProducto, 3, "Venta");

        // ASSERT: Verificar que el stock se decremento correctamente
        Assert.NotNull(resultado);
        Assert.Equal(7, resultado!.Stock);
    }

    /// <summary>
    /// Valida que AjustarStockAsync retorne null cuando el producto no existe
    /// </summary>
    [Fact]
    public async Task AjustarStockAsync_CuandoProductoNoExiste_DebeRetornarNull()
    {
        // ARRANGE: Repositorio retorna null para el producto
        Guid idProducto = Guid.NewGuid();

        _repositorioProductoMock
            .Setup(repositorio => repositorio.ObtenerProductoPorIdAsync(idProducto))
            .ReturnsAsync((ProductoEntidad?)null);

        ProductoServicio servicio = CrearServicio();

        // ACT: Intentar ajustar stock de un producto inexistente
        Sistema.Inventario.Producto.Aplicacion.DTOs.Responses.ProductoResponse? resultado = await servicio.AjustarStockAsync(idProducto, 5, "Venta");

        // ASSERT: Verificar que retorna null sin llamar al repositorio de actualizacion
        Assert.Null(resultado);
        _repositorioProductoMock.Verify(repositorio => repositorio.AjustarStockAsync(It.IsAny<Guid>(), It.IsAny<int>()), Times.Never);
    }

    /// <summary>
    /// Valida que AjustarStockAsync retorne null cuando el stock es insuficiente para una Venta
    /// </summary>
    [Fact]
    public async Task AjustarStockAsync_CuandoStockInsuficienteParaVenta_DebeRetornarNull()
    {
        // ARRANGE: Producto con stock 2, se intenta vender 5 unidades
        Guid idProducto = Guid.NewGuid();
        ProductoEntidad entidadExistente = new()
        {
            Id = idProducto,
            Nombre = "Cable HDMI",
            Descripcion = "Cable HDMI 2.0",
            Categoria = "Accesorios",
            ImagenUrl = "https://sistemainventario.com/productos/hdmi.png",
            Precio = 15m,
            Stock = 2
        };

        _repositorioProductoMock
            .Setup(repositorio => repositorio.ObtenerProductoPorIdAsync(idProducto))
            .ReturnsAsync(entidadExistente);

        ProductoServicio servicio = CrearServicio();

        // ACT: Intentar vender mas unidades de las disponibles
        Sistema.Inventario.Producto.Aplicacion.DTOs.Responses.ProductoResponse? resultado = await servicio.AjustarStockAsync(idProducto, 5, "Venta");

        // ASSERT: Verificar que retorna null sin intentar actualizar
        Assert.Null(resultado);
        _repositorioProductoMock.Verify(repositorio => repositorio.AjustarStockAsync(It.IsAny<Guid>(), It.IsAny<int>()), Times.Never);
    }
}
