namespace Sistema.Inventario.Storage.DTOs;

/// <summary>
/// Respuesta del archivo guardado en almacenamiento local.
/// </summary>
public class ArchivoSubidoResponse
{
    /// <summary>
    /// Nombre único del archivo almacenado.
    /// </summary>
    public string NombreArchivo { get; set; } = string.Empty;

    /// <summary>
    /// URL pública para consumir la imagen.
    /// </summary>
    public string UrlPublica { get; set; } = string.Empty;

    /// <summary>
    /// Tipo MIME del archivo.
    /// </summary>
    public string TipoContenido { get; set; } = string.Empty;

    /// <summary>
    /// Tamaño del archivo en bytes.
    /// </summary>
    public long TamanoBytes { get; set; }
}
