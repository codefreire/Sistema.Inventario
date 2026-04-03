using Microsoft.AspNetCore.Http;

namespace Sistema.Inventario.Storage.DTOs.Requests;

/// <summary>
/// Solicitud para subir un archivo imagen al almacenamiento
/// </summary>
public class ArchivoImagenRequest
{
    /// <summary>
    /// Archivo imagen a subir
    /// </summary>
    public IFormFile Archivo { get; set; }
}
