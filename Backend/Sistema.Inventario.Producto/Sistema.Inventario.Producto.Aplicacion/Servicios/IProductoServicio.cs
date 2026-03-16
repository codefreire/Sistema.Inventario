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
}