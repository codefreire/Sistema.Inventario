using Sistema.Inventario.Producto.Aplicacion.DTOs.Requests;
using Sistema.Inventario.Producto.Aplicacion.DTOs.Responses;

namespace Sistema.Inventario.Producto.Aplicacion.Servicios;

/// <summary>
/// Interface de servicio para manejar la lógica de negocio de Productos
/// </summary>
public interface IProductoServicio
{
    /// <summary>
    /// Método para obtener la lista de Productos
    /// </summary>
    /// <returns>Lista de productos</returns>
    Task<List<ProductoResponse>> ObtenerProductosAsync();

    /// <summary>
    /// Método para obtener un Producto por su Id
    /// </summary>
    /// <param name="id">Identificador del Producto</param>
    /// <returns>Producto encontrado o null si no existe</returns>
    Task<ProductoResponse?> ObtenerProductoPorIdAsync(Guid id);

    /// <summary>
    /// Método para crear un Producto
    /// </summary>
    /// <param name="request">Datos del Producto a crear</param>
    /// <returns>Producto creado</returns>
    Task<ProductoResponse> CrearProductoAsync(CrearProductoRequest request);

    /// <summary>
    /// Método para actualizar un Producto
    /// </summary>
    /// <param name="id">Identificador del Producto</param>
    /// <param name="request">Datos del Producto a actualizar</param>
    /// <returns>Producto actualizado o null si no existe</returns>
    Task<ProductoResponse?> ActualizarProductoAsync(Guid id, ActualizarProductoRequest request);

    /// <summary>
    /// Método para eliminar un Producto
    /// </summary>
    /// <param name="id">Identificador del Producto</param>
    /// <returns>True si fue eliminado, false si no existe</returns>
    Task<bool> EliminarProductoAsync(Guid id);
}