using Sistema.Inventario.Producto.Aplicacion.DTOs.Requests;
using Sistema.Inventario.Producto.Aplicacion.DTOs.Responses;
using Sistema.Inventario.Producto.Aplicacion.Servicios;

namespace Sistema.Inventario.Producto.Aplicacion.Handlers;

/// <summary>
/// Handler para actualizar un Producto
/// </summary>
public class ActualizarProductoHandler
{
    /// <summary>
    /// Servicio para manejar la lógica de negocio de Productos
    /// </summary>
    private readonly IProductoServicio _productoServicio;

    /// <summary>
    /// Constructor del handler para actualizar un Producto
    /// </summary>
    /// <param name="productoServicio">Servicio para manejar la lógica de negocio de Productos</param>
    public ActualizarProductoHandler(IProductoServicio productoServicio)
    {
        _productoServicio = productoServicio;
    }

    /// <summary>
    /// Método para manejar la actualización de un Producto
    /// </summary>
    /// <param name="id">Identificador del Producto</param>
    /// <param name="request">Datos del Producto a actualizar</param>
    /// <returns>Producto actualizado o null si no existe</returns>
    public async Task<ProductoResponse?> Handle(Guid id, ActualizarProductoRequest request)
    {
        return await _productoServicio.ActualizarProductoAsync(id, request);
    }
}