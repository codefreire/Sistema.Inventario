namespace Sistema.Inventario.Producto.Aplicacion.DTOs.Requests;

/// <summary>
/// Clase que representa la solicitud para crear un Producto
/// </summary>
public class CrearProductoRequest
{
    /// <summary>
    /// Nombre del Producto
    /// </summary>
    public string Nombre { get; set; }

    /// <summary>
    /// Descripción del Producto
    /// </summary>
    public string Descripcion { get; set; }

    /// <summary>
    /// Categoría del Producto
    /// </summary>
    public string Categoria { get; set; }

    /// <summary>
    /// Url de la imagen del Producto
    /// </summary>
    public string ImagenUrl { get; set; }

    /// <summary>
    /// El precio del Producto
    /// </summary>
    public decimal Precio { get; set; }

    /// <summary>
    /// Cantidad de Productos en stock
    /// </summary>
    public int Stock { get; set; }
}