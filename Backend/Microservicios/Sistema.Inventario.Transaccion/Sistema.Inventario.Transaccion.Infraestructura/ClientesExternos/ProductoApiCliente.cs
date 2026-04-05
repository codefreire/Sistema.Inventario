using Sistema.Inventario.Transaccion.Aplicacion.DTOs.Responses;
using Sistema.Inventario.Transaccion.Aplicacion.DTOs.Requests;

namespace Sistema.Inventario.Transaccion.Infraestructura.ClientesExternos;

/// <summary>
/// Clase que implementa la comunicación HTTP con el microservicio de Productos
/// </summary>
public class ProductoApiCliente : IProductoApiCliente
{
    /// <summary>
    /// Cliente HTTP para realizar las solicitudes al microservicio de Productos
    /// </summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Logger para registrar eventos de la comunicación con el microservicio de Productos
    /// </summary>
    private readonly ILogger<ProductoApiCliente> _logger;

    /// <summary>
    /// Constructor del cliente API de Productos
    /// </summary>
    /// <param name="httpClient">Cliente HTTP inyectado por el contenedor de dependencias</param>
    /// <param name="logger">Logger para registrar eventos</param>
    public ProductoApiCliente(HttpClient httpClient, ILogger<ProductoApiCliente> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene la información de un Producto por su Id desde el microservicio de Productos
    /// </summary>
    /// <param name="productoId">Identificador del Producto</param>
    /// <returns>Información del producto o null si no existe</returns>
    public async Task<ProductoExternoResponse?> ObtenerProductoPorIdAsync(Guid productoId)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"api/productos/{productoId}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"El microservicio de Productos respondió con código {response.StatusCode} para el producto con Id {productoId}.");
                return null;
            }

            return await response.Content.ReadFromJsonAsync<ProductoExternoResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al comunicarse con el microservicio de Productos para obtener el producto con Id {productoId}.");
            throw;
        }
    }

    /// <summary>
    /// Ajusta el stock de un Producto en el microservicio de Productos
    /// </summary>
    /// <param name="productoId">Identificador del Producto</param>
    /// <param name="cantidad">Cantidad a ajustar</param>
    /// <param name="tipoOperacion">Tipo de operación: Compra o Venta</param>
    /// <returns>True si el ajuste fue exitoso, false si falló</returns>
    public async Task<bool> AjustarStockAsync(Guid productoId, int cantidad, string tipoOperacion)
    {
        try
        {
            AjustarStockRequest requestBody = new AjustarStockRequest
            {
                Cantidad = cantidad,
                TipoOperacion = tipoOperacion
            };
            HttpResponseMessage response = await _httpClient.PatchAsJsonAsync($"api/productos/{productoId}/stock", requestBody);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"No se pudo ajustar el stock del producto con Id {productoId}. Código de respuesta: {response.StatusCode}.");
                return false;
            }

            _logger.LogInformation($"Stock ajustado exitosamente para el producto con Id {productoId}. Tipo: {tipoOperacion}, Cantidad: {cantidad}.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al comunicarse con el microservicio de Productos para ajustar el stock del producto con Id {productoId}.");
            throw;
        }
    }
}
