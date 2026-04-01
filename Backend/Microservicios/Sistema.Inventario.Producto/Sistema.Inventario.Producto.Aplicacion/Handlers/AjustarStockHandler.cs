using Sistema.Inventario.Producto.Aplicacion.DTOs.Requests;
using Sistema.Inventario.Producto.Aplicacion.DTOs.Responses;
using Sistema.Inventario.Producto.Aplicacion.Servicios;

namespace Sistema.Inventario.Producto.Aplicacion.Handlers;

/// <summary>
/// Handler para ajustar el stock de un Producto
/// </summary>
public class AjustarStockHandler
{
    /// <summary>
    /// Servicio para manejar la lógica de negocio de Productos
    /// </summary>
    private readonly IProductoServicio _productoServicio;

    /// <summary>
    /// Constructor del handler para ajustar el stock de un Producto
    /// </summary>
    /// <param name="productoServicio">Servicio para manejar la lógica de negocio de Productos</param>
    public AjustarStockHandler(IProductoServicio productoServicio)
    {
        _productoServicio = productoServicio;
    }

    /// <summary>
    /// Método para manejar el ajuste de stock de un Producto
    /// </summary>
    /// <param name="id">Identificador del Producto</param>
    /// <param name="request">Datos del ajuste de stock</param>
    /// <returns>Producto con stock ajustado o null si no existe</returns>
    public async Task<ProductoResponse?> Handle(Guid id, AjustarStockRequest request)
    {
        return await _productoServicio.AjustarStockAsync(id, request.Cantidad, request.TipoOperacion);
    }
}
