using Sistema.Inventario.Producto.Aplicacion.DTOs.Responses;
using Sistema.Inventario.Producto.Aplicacion.Servicios;

namespace Sistema.Inventario.Producto.Aplicacion.Handlers;

/// <summary>
/// Handler para obtener un Producto por su Id
/// </summary>
public class ObtenerProductoHandler
{
    /// <summary>
    /// Servicio para manejar la lógica de negocio de Productos
    /// </summary>
    private readonly IProductoServicio _productoServicio;

    /// <summary>
    /// Constructor del handler para obtener un Producto por su Id
    /// </summary>
    /// <param name="productoServicio">Servicio para manejar la lógica de negocio de Productos</param>
    public ObtenerProductoHandler(IProductoServicio productoServicio)
    {
        _productoServicio = productoServicio;
    }

    /// <summary>
    /// Método para manejar la obtención de un Producto por su Id
    /// </summary>
    /// <param name="id">Identificador único del Producto</param>
    /// <returns>Producto encontrado o null si no existe</returns>
    public async Task<ProductoResponse?> Handle(Guid id)
    {
        return await _productoServicio.ObtenerProductoPorIdAsync(id);
    }
}
