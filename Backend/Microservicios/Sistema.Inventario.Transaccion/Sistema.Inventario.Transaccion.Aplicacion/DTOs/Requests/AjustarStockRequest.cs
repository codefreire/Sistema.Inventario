namespace Sistema.Inventario.Transaccion.Aplicacion.DTOs.Requests;

/// <summary>
/// Request para el payload de ajuste de stock
/// </summary>
public class AjustarStockRequest
{
    /// <summary>
    /// Cantidad a ajustar (positiva para compras, negativa para ventas)
    /// </summary>
    public int Cantidad { get; init; }

    /// <summary>
    /// Tipo de operación: Compra o Venta
    /// </summary>
    public string TipoOperacion { get; init; }
}