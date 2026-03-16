namespace Sistema.Inventario.Producto.Aplicacion.DTOs.Responses;

/// <summary>
/// Clase que representa la respuesta de un Producto en la aplicación
/// </summary>
public class ProductoResponse
{
    /// <summary>
    /// Identificador único del Producto
    /// </summary>
    public Guid Id { get; set; }

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