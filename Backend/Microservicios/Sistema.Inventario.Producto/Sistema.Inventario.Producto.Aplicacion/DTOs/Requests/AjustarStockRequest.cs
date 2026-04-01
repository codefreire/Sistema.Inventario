namespace Sistema.Inventario.Producto.Aplicacion.DTOs.Requests;

/// <summary>
/// Clase que representa la solicitud para ajustar el stock de un Producto
/// </summary>
public class AjustarStockRequest
{
    /// <summary>
    /// Cantidad a ajustar en el stock
    /// </summary>
    public int Cantidad { get; set; }

    /// <summary>
    /// Tipo de operación: "Compra" incrementa el stock, "Venta" lo decrementa
    /// </summary>
    public string TipoOperacion { get; set; }
}
