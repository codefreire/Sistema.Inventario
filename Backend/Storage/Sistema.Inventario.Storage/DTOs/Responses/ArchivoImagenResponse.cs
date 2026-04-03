namespace Sistema.Inventario.Storage.DTOs.Responses;

/// <summary>
/// Respuesta del archivo imagen guardado en almacenamiento
/// </summary>
public class ArchivoImagenResponse
{
    /// <summary>
    /// Nombre único del archivo imagen almacenado
    /// </summary>
    public string NombreArchivo { get; set; }

    /// <summary>
    /// URL pública para consumir la imagen
    /// </summary>
    public string UrlImagen { get; set; }

    /// <summary>
    /// Tipo MIME del archivo imagen
    /// </summary>
    public string TipoContenido { get; set; }

    /// <summary>
    /// Tamaño del archivo imagen en bytes
    /// </summary>
    public long TamanioBytes { get; set; }
}
