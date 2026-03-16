using Microsoft.EntityFrameworkCore;
using Sistema.Inventario.Producto.Dominio.Entidades;
using Sistema.Inventario.Producto.Infraestructura.Repositorios;

namespace Sistema.Inventario.Producto.Infraestructura.Persistencia;

/// <summary>
/// Clase de repositorio para acceder a los datos de Productos en la base de datos
/// </summary>
public class ProductoRepositorio : IProductoRepositorio
{
    /// <summary>
    /// Contexto de la base de datos para acceder a los datos de Productos
    /// </summary>
    private readonly ProductoDbContext _contexto;

    /// <summary>
    /// Constructor del repositorio para acceder a los datos de Productos en la base de datos
    /// </summary>
    /// <param name="contexto">Contexto de la base de datos</param>
    public ProductoRepositorio(ProductoDbContext contexto)
    {
        _contexto = contexto;
    }

    /// <summary>
    /// Método para obtener la lista de Productos
    /// </summary>
    /// <returns>Lista de productos</returns>
    public async Task<List<ProductoEntidad>> ObtenerProductosAsync()
    {
        return await _contexto.Productos.AsNoTracking().ToListAsync();
    }
}