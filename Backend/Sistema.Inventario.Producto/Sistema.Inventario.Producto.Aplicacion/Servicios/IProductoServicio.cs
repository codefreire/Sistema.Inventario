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
}