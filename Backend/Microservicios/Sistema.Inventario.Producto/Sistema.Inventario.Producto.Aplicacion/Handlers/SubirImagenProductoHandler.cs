using Sistema.Inventario.Producto.Aplicacion.DTOs.Responses;

using Sistema.Inventario.Storage.DTOs.Requests;
using Sistema.Inventario.Storage.DTOs.Responses;
using Sistema.Inventario.Storage.Servicios;

namespace Sistema.Inventario.Producto.Aplicacion.Handlers;

/// <summary>
/// Handler para subir una imagen de Producto al almacenamiento
/// </summary>
public class SubirImagenProductoHandler
{
    /// <summary>
    /// Servicio de almacenamiento de archivos para manejar la lógica de subir imágenes
    /// </summary>
    private readonly IAlmacenamientoServicio _almacenamientoServicio;

    /// <summary>
    /// Constructor del handler para subir imágenes de Producto
    /// </summary>
    /// <param name="almacenamientoServicio">Servicio de almacenamiento de archivos para manejar la lógica de subir imágenes</param>
    public SubirImagenProductoHandler(IAlmacenamientoServicio almacenamientoServicio)
    {
        _almacenamientoServicio = almacenamientoServicio;
    }

    /// <summary>
    /// Sube un archivo de imagen y devuelve la URL resultante
    /// </summary>
    /// <param name="archivo">Archivo de imagen a subir</param>
    /// <returns>Respuesta con la URL de la imagen</returns>
    public async Task<SubirImagenResponse> Handle(IFormFile archivo)
    {
        ArchivoImagenResponse archivoSubido = await _almacenamientoServicio.GuardarArchivoAsync(new ArchivoImagenRequest
        {
            Archivo = archivo
        });

        return new SubirImagenResponse
        {
            ImagenUrl = archivoSubido.UrlImagen
        };
    }
}
