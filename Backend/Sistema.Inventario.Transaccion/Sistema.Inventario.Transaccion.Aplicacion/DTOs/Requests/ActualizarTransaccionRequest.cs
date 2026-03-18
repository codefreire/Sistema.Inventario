namespace Sistema.Inventario.Transaccion.Aplicacion.DTOs.Requests;

/// <summary>
/// Clase que representa la solicitud para actualizar una Transacción
/// </summary>
public class ActualizarTransaccionRequest
{
    /// <summary>
    /// Tipo de la Transacción (Compra o Venta)
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
    /// Detalle de la Transacción
    /// </summary>
    public string Detalle { get; set; }
}