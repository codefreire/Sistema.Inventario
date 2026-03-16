using Sistema.Inventario.Producto.Aplicacion.DTOs.Responses;
using Sistema.Inventario.Producto.Aplicacion.Servicios;

namespace Sistema.Inventario.Producto.Aplicacion.Handlers;

/// <summary>
/// Handler para obtener la lista de Productos
/// </summary>
public class ObtenerProductosHandler
{
    /// <summary>
    /// Servicio para manejar la lógica de negocio de Productos
    /// </summary>
    private readonly IProductoServicio _productoServicio;

    /// <summary>
    /// Constructor del handler para obtener la lista de Productos
    /// </summary>
    /// <param name="productoServicio">Servicio para manejar la lógica de negocio de Productos</param>
    public ObtenerProductosHandler(IProductoServicio productoServicio)
    {
        _productoServicio = productoServicio;
    }

    /// <summary>
    /// Método para manejar la obtención de la lista de Productos
    /// </summary>
    /// <returns>Lista de productos</returns>
    public async Task<List<ProductoResponse>> Handle()
    {
        return await _productoServicio.ObtenerProductosAsync();
    }
}