using Sistema.Inventario.Transaccion.Aplicacion.DTOs.Externos;
using Sistema.Inventario.Transaccion.Aplicacion.DTOs.Requests;
using Sistema.Inventario.Transaccion.Aplicacion.DTOs.Responses;
using Sistema.Inventario.Transaccion.Dominio.Entidades;
using Sistema.Inventario.Transaccion.Infraestructura.ClientesExternos;
using Sistema.Inventario.Transaccion.Infraestructura.Repositorios;

namespace Sistema.Inventario.Transaccion.Aplicacion.Servicios;

/// <summary>
/// Clase de servicio para manejar la lógica de negocio de Transacciones
/// </summary>
public class TransaccionServicio : ITransaccionServicio
{
    /// <summary>
    /// Repositorio para acceder a los datos de Transacciones
    /// </summary>
    private readonly ITransaccionRepositorio _transaccionRepositorio;

    /// <summary>
    /// Cliente para comunicarse con el microservicio de Productos
    /// </summary>
    private readonly IProductoApiCliente _productoApiCliente;

    /// <summary>
    /// Servicio de logging para mostrar eventos
    /// </summary>
    private readonly ILogger<TransaccionServicio> _logger;

    /// <summary>
    /// Constructor del servicio para manejar la lógica de negocio de Transacciones
    /// </summary>
    /// <param name="transaccionRepositorio">Repositorio para acceder a los datos de Transacciones</param>
    /// <param name="productoApiCliente">Cliente para comunicarse con el microservicio de Productos</param>
    /// <param name="logger">Servicio de logging para mostrar eventos</param>
    public TransaccionServicio(ITransaccionRepositorio transaccionRepositorio, IProductoApiCliente productoApiCliente, ILogger<TransaccionServicio> logger)
    {
        _transaccionRepositorio = transaccionRepositorio;
        _productoApiCliente = productoApiCliente;
        _logger = logger;
    }

    /// <summary>
    /// Método para obtener la lista de Transacciones
    /// </summary>
    /// <returns>Lista de transacciones</returns>
    public async Task<List<TransaccionResponse>> ObtenerTransaccionesAsync()
    {
        try
        {
            List<TransaccionEntidad> transacciones = await _transaccionRepositorio.ObtenerTransaccionesAsync();
            return transacciones.Select(transaccion => new TransaccionResponse
            {
                Id = transaccion.Id,
                Fecha = transaccion.Fecha,
                TipoTransaccion = transaccion.TipoTransaccion,
                ProductoId = transaccion.ProductoId,
                Cantidad = transaccion.Cantidad,
                PrecioUnitario = transaccion.PrecioUnitario,
                PrecioTotal = transaccion.PrecioTotal,
                Detalle = transaccion.Detalle
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la lista de transacciones.");
            throw;
        }
    }

    /// <summary>
    /// Método para obtener una Transacción por su Id
    /// </summary>
    /// <param name="id">Identificador de la Transacción</param>
    /// <returns>Transacción encontrada o null si no existe</returns>
    public async Task<TransaccionResponse?> ObtenerTransaccionPorIdAsync(Guid id)
    {
        try
        {
            TransaccionEntidad? transaccion = await _transaccionRepositorio.ObtenerTransaccionPorIdAsync(id);
            if (transaccion is null)
            {
                _logger.LogWarning($"No se encontró la transacción con Id {id}.");
                return null;
            }

            return new TransaccionResponse
            {
                Id = transaccion.Id,
                Fecha = transaccion.Fecha,
                TipoTransaccion = transaccion.TipoTransaccion,
                ProductoId = transaccion.ProductoId,
                Cantidad = transaccion.Cantidad,
                PrecioUnitario = transaccion.PrecioUnitario,
                PrecioTotal = transaccion.PrecioTotal,
                Detalle = transaccion.Detalle
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener la transacción con Id {id}.");
            throw;
        }
    }

    /// <summary>
    /// Método para crear una Transacción
    /// </summary>
    /// <param name="request">Datos de la Transacción a crear</param>
    /// <returns>Transacción creada</returns>
    public async Task<TransaccionResponse> CrearTransaccionAsync(CrearTransaccionRequest request)
    {
        try
        {
            // Comunicación síncrona: obtener producto desde el microservicio de Productos
            ProductoExternoResponse? producto = await _productoApiCliente.ObtenerProductoPorIdAsync(request.ProductoId);
            if (producto is null)
            {
                throw new InvalidOperationException($"El producto con Id {request.ProductoId} no existe en el sistema.");
            }

            // Validación de stock: si es una venta, verificar que haya stock suficiente
            if (request.TipoTransaccion.Equals("Venta", StringComparison.OrdinalIgnoreCase) && producto.Stock < request.Cantidad)
            {
                throw new InvalidOperationException($"Stock insuficiente para el producto '{producto.Nombre}'. Stock disponible: {producto.Stock}, cantidad solicitada: {request.Cantidad}.");
            }

            TransaccionEntidad transaccion = new()
            {
                Id = Guid.NewGuid(),
                Fecha = DateTime.Now,
                TipoTransaccion = request.TipoTransaccion,
                ProductoId = request.ProductoId,
                Cantidad = request.Cantidad,
                PrecioUnitario = request.PrecioUnitario,
                PrecioTotal = request.Cantidad * request.PrecioUnitario,
                Detalle = request.Detalle
            };

            await _transaccionRepositorio.CrearTransaccionAsync(transaccion);

            // Ajuste de stock: comunicación síncrona con el microservicio de Productos
            bool stockAjustado = await _productoApiCliente.AjustarStockAsync(request.ProductoId, request.Cantidad, request.TipoTransaccion);
            if (!stockAjustado)
            {
                _logger.LogWarning($"No se pudo ajustar el stock del producto con Id {request.ProductoId} después de crear la transacción {transaccion.Id}.");
            }

            _logger.LogInformation($"Transacción creada con Id {transaccion.Id} para el producto con Id {transaccion.ProductoId}.");

            return new TransaccionResponse
            {
                Id = transaccion.Id,
                Fecha = transaccion.Fecha,
                TipoTransaccion = transaccion.TipoTransaccion,
                ProductoId = transaccion.ProductoId,
                Cantidad = transaccion.Cantidad,
                PrecioUnitario = transaccion.PrecioUnitario,
                PrecioTotal = transaccion.PrecioTotal,
                Detalle = transaccion.Detalle
            };
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error inesperado al crear la transacción para el producto con Id {request.ProductoId}.");
            throw;
        }
    }

    /// <summary>
    /// Método para actualizar una Transacción
    /// </summary>
    /// <param name="id">Identificador de la Transacción</param>
    /// <param name="request">Datos de la Transacción a actualizar</param>
    /// <returns>Transacción actualizada o null si no existe</returns>
    public async Task<TransaccionResponse?> ActualizarTransaccionAsync(Guid id, ActualizarTransaccionRequest request)
    {
        try
        {
            // Obtener la transacción actual para revertir el efecto en stock
            TransaccionEntidad? transaccionActual = await _transaccionRepositorio.ObtenerTransaccionPorIdAsync(id);
            if (transaccionActual is null)
            {
                _logger.LogWarning($"No se encontró la transacción con Id {id} para actualizar.");
                return null;
            }

            // Validar que el producto nuevo existe
            ProductoExternoResponse? producto = await _productoApiCliente.ObtenerProductoPorIdAsync(request.ProductoId);
            if (producto is null)
            {
                throw new InvalidOperationException($"El producto con Id {request.ProductoId} no existe en el sistema.");
            }

            // Revertir el efecto de la transacción anterior en el stock
            string tipoReversion = transaccionActual.TipoTransaccion.Equals("Compra", StringComparison.OrdinalIgnoreCase) ? "Venta" : "Compra";
            await _productoApiCliente.AjustarStockAsync(transaccionActual.ProductoId, transaccionActual.Cantidad, tipoReversion);

            // Obtener el stock actualizado después de la reversión
            ProductoExternoResponse? productoActualizado = await _productoApiCliente.ObtenerProductoPorIdAsync(request.ProductoId);

            // Validar stock si la nueva transacción es una venta
            if (request.TipoTransaccion.Equals("Venta", StringComparison.OrdinalIgnoreCase) && productoActualizado != null && productoActualizado.Stock < request.Cantidad)
            {
                // Restaurar el stock original
                await _productoApiCliente.AjustarStockAsync(transaccionActual.ProductoId, transaccionActual.Cantidad, transaccionActual.TipoTransaccion);
                throw new InvalidOperationException($"Stock insuficiente para el producto '{producto.Nombre}'. Stock disponible: {productoActualizado.Stock}, cantidad solicitada: {request.Cantidad}.");
            }

            TransaccionEntidad datosActualizados = new()
            {
                TipoTransaccion = request.TipoTransaccion,
                ProductoId = request.ProductoId,
                Cantidad = request.Cantidad,
                PrecioUnitario = request.PrecioUnitario,
                PrecioTotal = request.Cantidad * request.PrecioUnitario,
                Detalle = request.Detalle
            };

            TransaccionEntidad? transaccion = await _transaccionRepositorio.ActualizarTransaccionAsync(id, datosActualizados);
            if (transaccion is null)
            {
                _logger.LogWarning($"No se encontró la transacción con Id {id} para actualizar.");
                return null;
            }

            // Aplicar el efecto de la nueva transacción en el stock
            await _productoApiCliente.AjustarStockAsync(request.ProductoId, request.Cantidad, request.TipoTransaccion);

            _logger.LogInformation($"Transacción con Id {id} actualizada correctamente.");

            return new TransaccionResponse
            {
                Id = transaccion.Id,
                Fecha = transaccion.Fecha,
                TipoTransaccion = transaccion.TipoTransaccion,
                ProductoId = transaccion.ProductoId,
                Cantidad = transaccion.Cantidad,
                PrecioUnitario = transaccion.PrecioUnitario,
                PrecioTotal = transaccion.PrecioTotal,
                Detalle = transaccion.Detalle
            };
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error inesperado al actualizar la transacción con Id {id}.");
            throw;
        }
    }

    /// <summary>
    /// Método para eliminar una Transacción
    /// </summary>
    /// <param name="id">Identificador de la Transacción</param>
    /// <returns>True si fue eliminada, false si no existe</returns>
    public async Task<bool> EliminarTransaccionAsync(Guid id)
    {
        try
        {
            // Obtener la transacción antes de eliminar para revertir el efecto en stock
            TransaccionEntidad? transaccion = await _transaccionRepositorio.ObtenerTransaccionPorIdAsync(id);
            if (transaccion is null)
            {
                _logger.LogWarning($"No se encontró la transacción con Id {id} para eliminar.");
                return false;
            }

            bool eliminado = await _transaccionRepositorio.EliminarTransaccionAsync(id);
            if (!eliminado)
            {
                _logger.LogWarning($"No se encontró la transacción con Id {id} para eliminar.");
                return false;
            }

            // Revertir el efecto en el stock: si fue Compra, decrementar; si fue Venta, incrementar
            string tipoReversion = transaccion.TipoTransaccion.Equals("Compra", StringComparison.OrdinalIgnoreCase) ? "Venta" : "Compra";
            await _productoApiCliente.AjustarStockAsync(transaccion.ProductoId, transaccion.Cantidad, tipoReversion);

            _logger.LogInformation($"Transacción con Id {id} eliminada correctamente.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error inesperado al eliminar la transacción con Id {id}.");
            throw;
        }
    }
}