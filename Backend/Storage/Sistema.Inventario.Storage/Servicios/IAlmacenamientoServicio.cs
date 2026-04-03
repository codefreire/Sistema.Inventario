using Sistema.Inventario.Storage.DTOs.Requests;
using Sistema.Inventario.Storage.DTOs.Responses;

namespace Sistema.Inventario.Storage.Servicios;

/// <summary>
/// Interface del servicio de almacenamiento de archivos imagenes
/// </summary>
public interface IAlmacenamientoServicio
{
    /// <summary>
    /// Guarda un archivo de imagen en el almacenamiento
    /// </summary>
    /// <param name="request">Solicitud con el archivo imagen a guardar</param>
    /// <returns>Datos del archivo imagen guardado</returns>
    Task<ArchivoImagenResponse> GuardarArchivoAsync(ArchivoImagenRequest request);

    /// <summary>
    /// Elimina un archivo de imagen del almacenamiento por su nombre único
    /// </summary>
    /// <param name="nombreArchivo">Nombre único del archivo imagen</param>
    /// <returns>True si fue eliminado o false si no se encontró</returns>
    Task<bool> EliminarArchivoAsync(string nombreArchivo);
}
