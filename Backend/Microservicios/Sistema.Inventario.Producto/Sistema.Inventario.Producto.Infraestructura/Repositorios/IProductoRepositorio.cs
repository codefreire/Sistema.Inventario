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

    /// <summary>
    /// Método para obtener un Producto por su Id
    /// </summary>
    /// <param name="id">Identificador del Producto</param>
    /// <returns>Producto encontrado o null si no existe</returns>
    Task<ProductoEntidad?> ObtenerProductoPorIdAsync(Guid id);

    /// <summary>
    /// Método para crear un Producto
    /// </summary>
    /// <param name="productoEntidad">Entidad del Producto a crear</param>
    Task CrearProductoAsync(ProductoEntidad productoEntidad);

    /// <summary>
    /// Método para actualizar un Producto
    /// </summary>
    /// <param name="id">Identificador del Producto</param>
    /// <param name="productoEntidad">Datos del Producto a actualizar</param>
    /// <returns>Producto actualizado o null si no existe</returns>
    Task<ProductoEntidad?> ActualizarProductoAsync(Guid id, ProductoEntidad productoEntidad);

    /// <summary>
    /// Método para actualizar únicamente el stock de un Producto
    /// </summary>
    /// <param name="id">Identificador del Producto</param>
    /// <param name="nuevoStock">Nuevo valor de stock</param>
    /// <returns>Producto actualizado o null si no existe</returns>
    Task<ProductoEntidad?> AjustarStockAsync(Guid id, int nuevoStock);

    /// <summary>
    /// Método para eliminar un Producto
    /// </summary>
    /// <param name="id">Identificador del Producto</param>
    /// <returns>True si fue eliminado, false si no existe</returns>
    Task<bool> EliminarProductoAsync(Guid id);
}