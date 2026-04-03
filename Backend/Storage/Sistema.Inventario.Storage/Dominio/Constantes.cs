namespace Sistema.Inventario.Storage.Dominio;

/// <summary>
/// Constantes utilizadas en el servicio de almacenamiento
/// </summary>
public static class Constantes
{
    /// <summary>
    /// Extension permitida para archivos imagen JPG
    /// </summary>
    public const string ExtensionJpg = ".jpg";

    /// <summary>
    /// Extension permitida para archivos imagen JPEG
    /// </summary>
    public const string ExtensionJpeg = ".jpeg";

    /// <summary>
    /// Extension permitida para archivos imagen PNG
    /// </summary>
    public const string ExtensionPng = ".png";

    /// <summary>
    /// Extension permitida para archivos imagen WEBP
    /// </summary>
    public const string ExtensionWebp = ".webp";

    /// <summary>
    /// Tamaño máximo permitido para los archivos imagen en bytes (5 MB)
    /// </summary>
    public const long TamanioMaximoBytes = 5L * 1024 * 1024;

    /// <summary>
    /// Carpeta dentro del directorio de almacenamiento donde se guardan las imágenes de los productos
    /// </summary>
    public const string CarpetaImagenes = "imagenes";
}