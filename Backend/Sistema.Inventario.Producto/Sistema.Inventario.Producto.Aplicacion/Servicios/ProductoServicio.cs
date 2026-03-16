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
}