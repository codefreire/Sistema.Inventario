using Microsoft.AspNetCore.Http;

namespace Sistema.Inventario.Storage.DTOs;

/// <summary>
/// Solicitud para subir un archivo al almacenamiento local.
/// </summary>
public class SubirArchivoRequest
{
    /// <summary>
    /// Archivo a subir.
    /// </summary>
    public IFormFile Archivo { get; set; } = null!;
}
