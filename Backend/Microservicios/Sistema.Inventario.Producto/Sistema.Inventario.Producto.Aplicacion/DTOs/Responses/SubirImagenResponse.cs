namespace Sistema.Inventario.Producto.Aplicacion.DTOs.Responses;

/// <summary>
/// Respuesta al subir una imagen de Producto.
/// </summary>
public class SubirImagenResponse
{
    /// <summary>
    /// URL pública de la imagen almacenada.
    /// </summary>
    public string ImagenUrl { get; set; } = string.Empty;
}
