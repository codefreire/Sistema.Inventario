using Sistema.Inventario.Storage.DTOs;

namespace Sistema.Inventario.Storage.Servicios;

/// <summary>
/// Contrato del servicio de almacenamiento de archivos.
/// </summary>
public interface IAlmacenamientoServicio
{
    /// <summary>
    /// Guarda un archivo en el almacenamiento local.
    /// </summary>
    /// <param name="solicitud">Solicitud con el archivo a guardar.</param>
    /// <returns>Datos del archivo guardado.</returns>
    Task<ArchivoSubidoResponse> GuardarArchivoAsync(SubirArchivoRequest solicitud);

    /// <summary>
    /// Elimina un archivo del almacenamiento local por su nombre único.
    /// </summary>
    /// <param name="nombreArchivo">Nombre único del archivo.</param>
    /// <returns>True si fue eliminado; false si no se encontró.</returns>
    Task<bool> EliminarArchivoAsync(string nombreArchivo);
}
