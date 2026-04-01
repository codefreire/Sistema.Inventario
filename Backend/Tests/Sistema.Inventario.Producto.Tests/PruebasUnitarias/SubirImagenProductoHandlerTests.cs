using Microsoft.AspNetCore.Http;
using Moq;
using Sistema.Inventario.Producto.Aplicacion.DTOs.Responses;
using Sistema.Inventario.Producto.Aplicacion.Handlers;
using Sistema.Inventario.Storage.DTOs;
using Sistema.Inventario.Storage.Servicios;

namespace Sistema.Inventario.Producto.Tests.PruebasUnitarias;

/// <summary>
/// Pruebas unitarias para SubirImagenProductoHandler usando Moq para el servicio de almacenamiento
/// </summary>
public class SubirImagenProductoHandlerTests
{
    /// <summary>
    /// Mock del servicio de almacenamiento
    /// </summary>
    private readonly Mock<IAlmacenamientoServicio> _almacenamientoServicioMock = new();

    /// <summary>
    /// Crea una instancia de SubirImagenProductoHandler con el mock configurado
    /// </summary>
    private SubirImagenProductoHandler CrearHandler()
    {
        return new SubirImagenProductoHandler(_almacenamientoServicioMock.Object);
    }

    /// <summary>
    /// Valida que el handler retorne la URL pública devuelta por el servicio de almacenamiento
    /// </summary>
    [Fact]
    public async Task Handle_CuandoArchivoValido_DebeRetornarUrlPublica()
    {
        // ARRANGE: Configurar mock del archivo y del servicio de almacenamiento
        Mock<IFormFile> archivoMock = new();
        archivoMock.Setup(a => a.FileName).Returns("imagen.jpg");
        archivoMock.Setup(a => a.Length).Returns(1024);

        string urlEsperada = "http://localhost:5261/uploads/abc123.jpg";

        _almacenamientoServicioMock
            .Setup(s => s.GuardarArchivoAsync(It.IsAny<SubirArchivoRequest>()))
            .ReturnsAsync(new ArchivoSubidoResponse
            {
                NombreArchivo = "abc123.jpg",
                UrlPublica = urlEsperada,
                TipoContenido = "image/jpeg",
                TamanoBytes = 1024
            });

        SubirImagenProductoHandler handler = CrearHandler();

        // ACT: Llamar al handler con el archivo mock
        SubirImagenResponse resultado = await handler.Handle(archivoMock.Object);

        // ASSERT: Verificar que la URL retornada coincide con la del servicio
        Assert.NotNull(resultado);
        Assert.Equal(urlEsperada, resultado.ImagenUrl);
    }

    /// <summary>
    /// Valida que el handler propague la excepción cuando el servicio de almacenamiento lanza ArgumentException
    /// </summary>
    [Fact]
    public async Task Handle_CuandoServicioLanzaExcepcion_DebePropagar()
    {
        // ARRANGE: Configurar mock del archivo y el servicio para lanzar excepción
        Mock<IFormFile> archivoMock = new();
        archivoMock.Setup(a => a.FileName).Returns("virus.exe");
        archivoMock.Setup(a => a.Length).Returns(512);

        _almacenamientoServicioMock
            .Setup(s => s.GuardarArchivoAsync(It.IsAny<SubirArchivoRequest>()))
            .ThrowsAsync(new ArgumentException("La extensión del archivo no está permitida. Use jpg, jpeg, png o webp."));

        SubirImagenProductoHandler handler = CrearHandler();

        // ACT & ASSERT: Verificar que la excepción se propaga al llamador
        ArgumentException excepcion = await Assert.ThrowsAsync<ArgumentException>(
            () => handler.Handle(archivoMock.Object));

        Assert.Contains("extensión", excepcion.Message);
    }

    /// <summary>
    /// Valida que el handler invoque GuardarArchivoAsync exactamente una vez con el archivo recibido
    /// </summary>
    [Fact]
    public async Task Handle_DebeInvocarGuardarArchivoAsyncUnaVez()
    {
        // ARRANGE: Configurar mock y respuesta del servicio
        Mock<IFormFile> archivoMock = new();
        archivoMock.Setup(a => a.FileName).Returns("foto.png");
        archivoMock.Setup(a => a.Length).Returns(2048);

        _almacenamientoServicioMock
            .Setup(s => s.GuardarArchivoAsync(It.IsAny<SubirArchivoRequest>()))
            .ReturnsAsync(new ArchivoSubidoResponse
            {
                NombreArchivo = "foto.png",
                UrlPublica = "http://localhost:5261/uploads/foto.png",
                TipoContenido = "image/png",
                TamanoBytes = 2048
            });

        SubirImagenProductoHandler handler = CrearHandler();

        // ACT: Llamar al handler
        await handler.Handle(archivoMock.Object);

        // ASSERT: Verificar que el servicio fue invocado exactamente una vez
        _almacenamientoServicioMock.Verify(
            s => s.GuardarArchivoAsync(It.Is<SubirArchivoRequest>(r => r.Archivo == archivoMock.Object)),
            Times.Once);
    }
}
