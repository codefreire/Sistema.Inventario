using Sistema.Inventario.Producto.Dominio.Entidades;

namespace Sistema.Inventario.Producto.Infraestructura.Repositorios;

/// <summary>
/// Interface de repositorio para acceder a los datos de Productos en la base de datos
/// </summary>
public interface IProductoRepositorio
{
    /// <summary>
    /// Método para obtener la lista de Productos
    /// </summary>
    /// <returns>Lista de productos</returns>
    Task<List<ProductoEntidad>> ObtenerProductosAsync();
}