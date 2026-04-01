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
    /// Servicio de logging para mostrar eventos
    /// </summary>
    private readonly ILogger<ProductoServicio> _logger;

    /// <summary>
    /// Constructor del servicio para manejar la lógica de negocio de Productos
    /// </summary>
    /// <param name="productoRepositorio">Repositorio para acceder a los datos de Productos</param>
    /// <param name="logger">Logging para registrar eventos del servicio</param>
    public ProductoServicio(IProductoRepositorio productoRepositorio, ILogger<ProductoServicio> logger)
    {
        _productoRepositorio = productoRepositorio;
        _logger = logger;
    }

    /// <summary>
    /// Método para obtener la lista de Productos
    /// </summary>
    /// <returns>Lista de productos</returns>
    public async Task<List<ProductoResponse>> ObtenerProductosAsync()
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al obtener la lista de productos.");
            throw;
        }
    }

    /// <summary>
    /// Método para obtener un Producto por su Id
    /// </summary>
    /// <param name="id">Identificador del Producto</param>
    /// <returns>Producto encontrado o null si no existe</returns>
    public async Task<ProductoResponse?> ObtenerProductoPorIdAsync(Guid id)
    {
        try
        {
            ProductoEntidad? producto = await _productoRepositorio.ObtenerProductoPorIdAsync(id);
            if (producto is null)
            {
                _logger.LogWarning($"No se encontro el producto con Id {id}.");
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
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error inesperado al obtener el producto con Id {id}.");
            throw;
        }
    }

    /// <summary>
    /// Método para crear un Producto
    /// </summary>
    /// <param name="request">Datos del Producto a crear</param>
    /// <returns>Producto creado</returns>
    public async Task<ProductoResponse> CrearProductoAsync(CrearProductoRequest request)
    {
        try
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

            _logger.LogInformation($"Producto creado correctamente con Id {producto.Id}.");

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
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error inesperado al crear el producto {request.Nombre}.");
            throw;
        }
    }

    /// <summary>
    /// Método para actualizar un Producto
    /// </summary>
    /// <param name="id">Identificador del Producto</param>
    /// <param name="request">Datos del Producto a actualizar</param>
    /// <returns>Producto actualizado o null si no existe</returns>
    public async Task<ProductoResponse?> ActualizarProductoAsync(Guid id, ActualizarProductoRequest request)
    {
        try
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
                _logger.LogWarning($"No se encontro el producto con Id {id} para actualizar.");
                return null;
            }

            _logger.LogInformation($"Producto actualizado correctamente con Id {id}.");

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
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error inesperado al actualizar el producto con Id {id}.");
            throw;
        }
    }

    /// <summary>
    /// Método para eliminar un Producto
    /// </summary>
    /// <param name="id">Identificador del Producto</param>
    /// <returns>True si fue eliminado, false si no existe</returns>
    public async Task<bool> EliminarProductoAsync(Guid id)
    {
        try
        {
            bool eliminado = await _productoRepositorio.EliminarProductoAsync(id);
            if (!eliminado)
            {
                _logger.LogWarning($"No se encontro el producto con Id {id} para eliminar.");
                return false;
            }

            _logger.LogInformation($"Producto eliminado correctamente con Id {id}.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error inesperado al eliminar el producto con Id {id}.");
            throw;
        }
    }

    /// <summary>
    /// Método para ajustar el stock de un Producto
    /// </summary>
    /// <param name="id">Identificador del Producto</param>
    /// <param name="cantidad">Cantidad a ajustar</param>
    /// <param name="tipoOperacion">Tipo de operación: Compra (incrementar) o Venta (decrementar)</param>
    /// <returns>Producto con stock ajustado o null si no existe</returns>
    public async Task<ProductoResponse?> AjustarStockAsync(Guid id, int cantidad, string tipoOperacion)
    {
        try
        {
            ProductoEntidad? producto = await _productoRepositorio.ObtenerProductoPorIdAsync(id);
            if (producto is null)
            {
                _logger.LogWarning($"No se encontró el producto con Id {id} para ajustar stock.");
                return null;
            }

            int nuevoStock = tipoOperacion.Equals("Compra", StringComparison.OrdinalIgnoreCase)
                ? producto.Stock + cantidad
                : producto.Stock - cantidad;

            if (nuevoStock < 0)
            {
                _logger.LogWarning($"Stock insuficiente para el producto con Id {id}. Stock actual: {producto.Stock}, cantidad solicitada: {cantidad}.");
                return null;
            }

            ProductoEntidad datosActualizados = new()
            {
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Categoria = producto.Categoria,
                ImagenUrl = producto.ImagenUrl,
                Precio = producto.Precio,
                Stock = nuevoStock
            };

            ProductoEntidad? productoActualizado = await _productoRepositorio.ActualizarProductoAsync(id, datosActualizados);
            if (productoActualizado is null)
            {
                return null;
            }

            _logger.LogInformation($"Stock ajustado para producto con Id {id}. Tipo: {tipoOperacion}, Cantidad: {cantidad}, Nuevo stock: {nuevoStock}.");

            return new ProductoResponse
            {
                Id = productoActualizado.Id,
                Nombre = productoActualizado.Nombre,
                Descripcion = productoActualizado.Descripcion,
                Categoria = productoActualizado.Categoria,
                ImagenUrl = productoActualizado.ImagenUrl,
                Precio = productoActualizado.Precio,
                Stock = productoActualizado.Stock
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error inesperado al ajustar el stock del producto con Id {id}.");
            throw;
        }
    }
}