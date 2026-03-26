using Microsoft.Extensions.Logging;
using Moq;
using Sistema.Inventario.Transaccion.Aplicacion.DTOs.Requests;
using Sistema.Inventario.Transaccion.Aplicacion.DTOs.Responses;
using Sistema.Inventario.Transaccion.Aplicacion.Servicios;
using Sistema.Inventario.Transaccion.Dominio.Entidades;
using Sistema.Inventario.Transaccion.Infraestructura.Repositorios;

namespace Sistema.Inventario.Transaccion.Tests.PruebasUnitarias;

/// <summary>
/// Pruebas unitarias para TransaccionServicio usando Moq para aislar dependencias
/// </summary>
public class TransaccionServicioTests
{
    /// <summary>
    /// Mock para el repositorio de transacciones
    /// </summary>
    private readonly Mock<ITransaccionRepositorio> _repositorioTransaccionMock = new();

    /// <summary>
    /// Mock para el servicio de logging
    /// </summary>
    private readonly Mock<ILogger<TransaccionServicio>> _loggerMock = new();

    /// <summary>
    /// Crea una instancia de TransaccionServicio con los mocks configurados
    /// </summary>
    /// <returns></returns>
    private TransaccionServicio CrearServicio()
    {
        return new TransaccionServicio(_repositorioTransaccionMock.Object, _loggerMock.Object);
    }

    /// <summary>
    /// Valida que el servicio mapee correctamente las transacciones obtenidas del repositorio
    /// </summary>
    [Fact]
    public async Task ObtenerTransaccionesAsync_CuandoExistenRegistros_MapeaTransacciones()
    {
        // ARRANGE: Preparar datos de transacciones y configurar el mock del repositorio
        List<TransaccionEntidad> transaccionesEntidad =
        [
            new TransaccionEntidad
            {
                Id = Guid.NewGuid(),
                Fecha = new DateTime(2026, 1, 5, 10, 30, 0),
                TipoTransaccion = "Compra",
                ProductoId = Guid.NewGuid(),
                Cantidad = 4,
                PrecioUnitario = 20.5m,
                PrecioTotal = 82m,
                Detalle = "Compra para bodega"
            },
            new TransaccionEntidad
            {
                Id = Guid.NewGuid(),
                Fecha = new DateTime(2026, 1, 8, 15, 0, 0),
                TipoTransaccion = "Venta",
                ProductoId = Guid.NewGuid(),
                Cantidad = 2,
                PrecioUnitario = 40m,
                PrecioTotal = 80m,
                Detalle = "Venta mostrador"
            }
        ];

        _repositorioTransaccionMock
            .Setup(repositorio => repositorio.ObtenerTransaccionesAsync())
            .ReturnsAsync(transaccionesEntidad);

        TransaccionServicio servicio = CrearServicio();

        // ACT: Obtener transacciones desde el servicio.
        List<TransaccionResponse> resultado = await servicio.ObtenerTransaccionesAsync();

        // ASSERT: Verificar cantidad y mapeo de datos principales.
        Assert.Equal(2, resultado.Count);
        Assert.Collection(resultado,
            transaccion =>
            {
                Assert.Equal(transaccionesEntidad[0].Id, transaccion.Id);
                Assert.Equal("Compra", transaccion.TipoTransaccion);
                Assert.Equal(82m, transaccion.PrecioTotal);
            },
            transaccion =>
            {
                Assert.Equal(transaccionesEntidad[1].Id, transaccion.Id);
                Assert.Equal("Venta", transaccion.TipoTransaccion);
                Assert.Equal(80m, transaccion.PrecioTotal);
            });
    }

    /// <summary>
    /// Valida que el servicio retorne null cuando no existe la transacción consultada
    /// </summary>
    [Fact]
    public async Task ObtenerTransaccionPorIdAsync_CuandoNoExiste_RetornaNull()
    {
        // ARRANGE: Preparar identificador de prueba y configurar retorno nulo en repositorio
        Guid idTransaccion = Guid.NewGuid();

        _repositorioTransaccionMock
            .Setup(repositorio => repositorio.ObtenerTransaccionPorIdAsync(idTransaccion))
            .ReturnsAsync((TransaccionEntidad?)null);

        TransaccionServicio servicio = CrearServicio();

        // ACT: Intentar obtener una transacción inexistente
        TransaccionResponse? resultado = await servicio.ObtenerTransaccionPorIdAsync(idTransaccion);

        // ASSERT: Verificar que el resultado sea nulo
        Assert.Null(resultado);
    }

    /// <summary>
    /// Valida que el servicio persista y retorne correctamente una nueva transacción
    /// </summary>
    [Fact]
    public async Task CrearTransaccionAsync_CuandoLaSolicitudEsValida_RetornaTransaccion()
    {
        // ARRANGE: Preparar solicitud de creación y capturar entidad enviada al repositorio
        CrearTransaccionRequest request = new()
        {
            TipoTransaccion = "Compra",
            ProductoId = Guid.NewGuid(),
            Cantidad = 6,
            PrecioUnitario = 9.5m,
            Detalle = "Compra semanal"
        };

        TransaccionEntidad? transaccionCreada = null;

        _repositorioTransaccionMock
            .Setup(repositorio => repositorio.CrearTransaccionAsync(It.IsAny<TransaccionEntidad>()))
            .Callback<TransaccionEntidad>(transaccion => transaccionCreada = transaccion)
            .Returns(Task.CompletedTask);

        TransaccionServicio servicio = CrearServicio();

        // ACT: Crear la transacción mediante el servicio.
        TransaccionResponse resultado = await servicio.CrearTransaccionAsync(request);

        // ASSERT: Verificar persistencia y cálculo correcto del precio total.
        Assert.NotNull(transaccionCreada);
        Assert.NotEqual(Guid.Empty, transaccionCreada!.Id);
        Assert.Equal(request.TipoTransaccion, transaccionCreada.TipoTransaccion);
        Assert.Equal(request.ProductoId, transaccionCreada.ProductoId);
        Assert.Equal(request.Cantidad, transaccionCreada.Cantidad);
        Assert.Equal(request.PrecioUnitario, transaccionCreada.PrecioUnitario);
        Assert.Equal(57m, transaccionCreada.PrecioTotal);
        Assert.Equal(request.Detalle, transaccionCreada.Detalle);

        Assert.Equal(transaccionCreada.Id, resultado.Id);
        Assert.Equal(57m, resultado.PrecioTotal);

        _repositorioTransaccionMock.Verify(repositorio => repositorio.CrearTransaccionAsync(It.IsAny<TransaccionEntidad>()), Times.Once);
    }

    /// <summary>
    /// Valida que el servicio actualice y retorne correctamente la transacción existente
    /// </summary>
    [Fact]
    public async Task ActualizarTransaccionAsync_CuandoLaTransaccionExiste_RetornaTransaccionActualizada()
    {
        // ARRANGE: Preparar identificador, solicitud de actualización y comportamiento del repositorio
        Guid idTransaccion = Guid.NewGuid();
        DateTime fechaExistente = new DateTime(2026, 2, 10, 9, 45, 0);

        ActualizarTransaccionRequest request = new()
        {
            TipoTransaccion = "Venta",
            ProductoId = Guid.NewGuid(),
            Cantidad = 3,
            PrecioUnitario = 18m,
            Detalle = "Venta actualizada"
        };

        _repositorioTransaccionMock
            .Setup(repositorio => repositorio.ActualizarTransaccionAsync(idTransaccion, It.IsAny<TransaccionEntidad>()))
            .ReturnsAsync((Guid id, TransaccionEntidad transaccionActualizada) =>
            {
                return new TransaccionEntidad
                {
                    Id = id,
                    Fecha = fechaExistente,
                    TipoTransaccion = transaccionActualizada.TipoTransaccion,
                    ProductoId = transaccionActualizada.ProductoId,
                    Cantidad = transaccionActualizada.Cantidad,
                    PrecioUnitario = transaccionActualizada.PrecioUnitario,
                    PrecioTotal = transaccionActualizada.PrecioTotal,
                    Detalle = transaccionActualizada.Detalle
                };
            });

        TransaccionServicio servicio = CrearServicio();

        // ACT: Actualizar la transacción en el servicio
        TransaccionResponse? resultado = await servicio.ActualizarTransaccionAsync(idTransaccion, request);

        // ASSERT: Verificar que la transacción se retorne con cambios y total calculado
        Assert.NotNull(resultado);
        Assert.Equal(idTransaccion, resultado!.Id);
        Assert.Equal(fechaExistente, resultado.Fecha);
        Assert.Equal("Venta", resultado.TipoTransaccion);
        Assert.Equal(54m, resultado.PrecioTotal);
        Assert.Equal("Venta actualizada", resultado.Detalle);
    }

    /// <summary>
    /// Valida que el servicio retorne falso cuando la transacción a eliminar no existe
    /// </summary>
    [Fact]
    public async Task EliminarTransaccionAsync_CuandoLaTransaccionNoExiste_RetornaFalso()
    {
        // ARRANGE: Preparar identificador de prueba y configurar eliminación fallida
        Guid idTransaccion = Guid.NewGuid();

        _repositorioTransaccionMock
            .Setup(repositorio => repositorio.EliminarTransaccionAsync(idTransaccion))
            .ReturnsAsync(false);

        TransaccionServicio servicio = CrearServicio();

        // ACT: Ejecutar eliminación de una transacción inexistente
        bool resultado = await servicio.EliminarTransaccionAsync(idTransaccion);

        // ASSERT: Verificar que el servicio retorne falso
        Assert.False(resultado);
    }
}
