namespace Sistema.Inventario.Transaccion.Aplicacion.DTOs.Responses;

/// <summary>
/// Clase que representa la respuesta de una Transacción en la aplicación
/// </summary>
public class TransaccionResponse
{
    /// <summary>
    /// Identificador único de la Transacción
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Fecha de la Transacción
    /// </summary>
    public DateTime Fecha { get; set; }

    /// <summary>
    /// Tipo de la Transacción
    /// </summary>
    public string TipoTransaccion { get; set; }

    /// <summary>
    /// Identificador del Producto asociado a la Transacción
    /// </summary>
    public Guid ProductoId { get; set; }

    /// <summary>
    /// Cantidad de Productos de la Transacción
    /// </summary>
    public int Cantidad { get; set; }

    /// <summary>
    /// Precio unitario del Producto en la Transacción
    /// </summary>
    public decimal PrecioUnitario { get; set; }

    /// <summary>
    /// Precio total del Producto en la Transacción
    /// </summary>
    public decimal PrecioTotal { get; set; }

    /// <summary>
    /// Detalle de la Transacción
    /// </summary>
    public string Detalle { get; set; }

}