using Microsoft.AspNetCore.Http;
using Sistema.Inventario.Producto.Aplicacion.DTOs.Responses;
using Sistema.Inventario.Storage.DTOs;
using Sistema.Inventario.Storage.Servicios;

namespace Sistema.Inventario.Producto.Aplicacion.Handlers;

/// <summary>
/// Handler para subir una imagen de Producto al almacenamiento local.
/// </summary>
public class SubirImagenProductoHandler
{
    /// <summary>
    /// Servicio de almacenamiento de archivos.
    /// </summary>
    private readonly IAlmacenamientoServicio _almacenamientoServicio;

    /// <summary>
    /// Constructor del handler para subir imágenes de Producto.
    /// </summary>
    /// <param name="almacenamientoServicio">Servicio de almacenamiento de archivos.</param>
    public SubirImagenProductoHandler(IAlmacenamientoServicio almacenamientoServicio)
    {
        _almacenamientoServicio = almacenamientoServicio;
    }

    /// <summary>
    /// Sube un archivo de imagen y devuelve la URL pública resultante.
    /// </summary>
    /// <param name="archivo">Archivo de imagen a subir.</param>
    /// <returns>Respuesta con la URL pública de la imagen.</returns>
    public async Task<SubirImagenResponse> Handle(IFormFile archivo)
    {
        ArchivoSubidoResponse archivoSubido = await _almacenamientoServicio.GuardarArchivoAsync(new SubirArchivoRequest
        {
            Archivo = archivo
        });

        return new SubirImagenResponse
        {
            ImagenUrl = archivoSubido.UrlPublica
        };
    }
}
