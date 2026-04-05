namespace Sistema.Inventario.Transaccion.Aplicacion.DTOs.Responses;

/// <summary>
/// Clase que representa la respuesta del microservicio de Productos
/// </summary>
public class ProductoExternoResponse
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
    /// Cantidad de Productos en stock
    /// </summary>
    public int Stock { get; set; }

    /// <summary>
    /// El precio del Producto
    /// </summary>
    public decimal Precio { get; set; }
}
