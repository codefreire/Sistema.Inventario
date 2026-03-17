using Sistema.Inventario.Producto.Aplicacion.DTOs.Requests;
using Sistema.Inventario.Producto.Aplicacion.DTOs.Responses;
using Sistema.Inventario.Producto.Dominio.Entidades;
using Sistema.Inventario.Producto.Infraestructura.Repositorios;

namespace Sistema.Inventario.Producto.Aplicacion.Servicios;

/// <summary>
/// Clase de servicio para manejar la lógica de negocio de Productos
/// </summary>
public class ProductoServicio : IProductoServicio
{
    /// <summary>
    /// Repositorio para acceder a los datos de Productos
    /// </summary>
    private readonly IProductoRepositorio _productoRepositorio;

    /// <summary>
    /// Constructor del servicio para manejar la lógica de negocio de Productos
    /// </summary>
    /// <param name="productoRepositorio">Repositorio para acceder a los datos de Productos</param>
    public ProductoServicio(IProductoRepositorio productoRepositorio)
    {
        _productoRepositorio = productoRepositorio;
    }

    /// <summary>
    /// Método para obtener la lista de Productos
    /// </summary>
    /// <returns>Lista de productos</returns>
    public async Task<List<ProductoResponse>> ObtenerProductosAsync()
    {
        List<ProductoEntidad> productos = await _productoRepositorio.ObtenerProductosAsync();
        return productos.Select(producto => new ProductoResponse
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            Categoria = producto.Categoria,
            ImagenUrl = producto.ImagenUrl,
            Precio = producto.Precio,
            Stock = producto.Stock
        }).ToList();
    }

    /// <summary>
    /// Método para obtener un Producto por su Id
    /// </summary>
    /// <param name="id">Identificador del Producto</param>
    /// <returns>Producto encontrado o null si no existe</returns>
    public async Task<ProductoResponse?> ObtenerProductoPorIdAsync(Guid id)
    {
        ProductoEntidad? producto = await _productoRepositorio.ObtenerProductoPorIdAsync(id);
        if (producto is null)
        {
            return null;
        }
        return new ProductoResponse
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            Categoria = producto.Categoria,
            ImagenUrl = producto.ImagenUrl,
            Precio = producto.Precio,
            Stock = producto.Stock
        };
    }

    /// <summary>
    /// Método para crear un Producto
    /// </summary>
    /// <param name="request">Datos del Producto a crear</param>
    /// <returns>Producto creado</returns>
    public async Task<ProductoResponse> CrearProductoAsync(CrearProductoRequest request)
    {
        ProductoEntidad producto = new()
        {
            Id = Guid.NewGuid(),
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            Categoria = request.Categoria,
            ImagenUrl = request.ImagenUrl,
            Precio = request.Precio,
            Stock = request.Stock
        };

        await _productoRepositorio.CrearProductoAsync(producto);

        return new ProductoResponse
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            Categoria = producto.Categoria,
            ImagenUrl = producto.ImagenUrl,
            Precio = producto.Precio,
            Stock = producto.Stock
        };
    }

    /// <summary>
    /// Método para actualizar un Producto
    /// </summary>
    /// <param name="id">Identificador del Producto</param>
    /// <param name="request">Datos del Producto a actualizar</param>
    /// <returns>Producto actualizado o null si no existe</returns>
    public async Task<ProductoResponse?> ActualizarProductoAsync(Guid id, ActualizarProductoRequest request)
    {
        ProductoEntidad datosActualizados = new()
        {
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            Categoria = request.Categoria,
            ImagenUrl = request.ImagenUrl,
            Precio = request.Precio,
            Stock = request.Stock
        };

        ProductoEntidad? producto = await _productoRepositorio.ActualizarProductoAsync(id, datosActualizados);
        if (producto is null)
        {
            return null;
        }

        return new ProductoResponse
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            Categoria = producto.Categoria,
            ImagenUrl = producto.ImagenUrl,
            Precio = producto.Precio,
            Stock = producto.Stock
        };
    }

    /// <summary>
    /// Método para eliminar un Producto
    /// </summary>
    /// <param name="id">Identificador del Producto</param>
    /// <returns>True si fue eliminado, false si no existe</returns>
    public async Task<bool> EliminarProductoAsync(Guid id)
    {
        return await _productoRepositorio.EliminarProductoAsync(id);
    }
}