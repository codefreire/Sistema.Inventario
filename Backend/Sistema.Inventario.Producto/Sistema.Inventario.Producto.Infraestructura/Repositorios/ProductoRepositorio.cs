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

    /// <summary>
    /// Método para obtener un Producto por su Id
    /// </summary>
    /// <param name="id">Identificador único del Producto</param>
    /// <returns>Producto encontrado o null si no existe</returns>
    public async Task<ProductoEntidad?> ObtenerProductoPorIdAsync(Guid id)
    {
        return await _contexto.Productos.AsNoTracking().FirstOrDefaultAsync(producto => producto.Id == id);
    }

    /// <summary>
    /// Método para crear un Producto
    /// </summary>
    /// <param name="producto">Entidad del Producto a crear</param>
    public async Task CrearProductoAsync(ProductoEntidad producto)
    {
        await _contexto.Productos.AddAsync(producto);
        await _contexto.SaveChangesAsync();
    }
}