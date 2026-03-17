using Sistema.Inventario.Producto.Aplicacion.DTOs.Requests;
using Sistema.Inventario.Producto.Aplicacion.Servicios;

namespace Sistema.Inventario.Producto.Aplicacion.Handlers;

/// <summary>
/// Handler para eliminar un Producto
/// </summary>
public class EliminarProductoHandler
{
    /// <summary>
    /// Servicio para manejar la lógica de negocio de Productos
    /// </summary>
    private readonly IProductoServicio _productoServicio;

    /// <summary>
    /// Constructor del handler para eliminar un Producto
    /// </summary>
    /// <param name="productoServicio">Servicio para manejar la lógica de negocio de Productos</param>
    public EliminarProductoHandler(IProductoServicio productoServicio)
    {
        _productoServicio = productoServicio;
    }

    /// <summary>
    /// Método para manejar la eliminación de un Producto
    /// </summary>
    /// <param name="id">Identificador del Producto</param>
    /// <returns>True si fue eliminado, false si no existe</returns>
    public async Task<bool> Handle(Guid id)
    {
        return await _productoServicio.EliminarProductoAsync(id);
    }
}