using Sistema.Inventario.Transaccion.Aplicacion.DTOs.Responses;

namespace Sistema.Inventario.Transaccion.Infraestructura.ClientesExternos;

/// <summary>
/// Interface para la comunicación con el microservicio de Productos
/// </summary>
public interface IProductoApiCliente
{
    /// <summary>
    /// Obtiene la información de un Producto por su Id desde el microservicio de Productos
    /// </summary>
    /// <param name="productoId">Identificador del Producto</param>
    /// <returns>Información del producto o null si no existe</returns>
    Task<ProductoExternoResponse?> ObtenerProductoPorIdAsync(Guid productoId);

    /// <summary>
    /// Ajusta el stock de un Producto en el microservicio de Productos
    /// </summary>
    /// <param name="productoId">Identificador del Producto</param>
    /// <param name="cantidad">Cantidad a ajustar</param>
    /// <param name="tipoOperacion">Tipo de operación: Compra o Venta</param>
    /// <returns>True si el ajuste fue exitoso, false si falló</returns>
    Task<bool> AjustarStockAsync(Guid productoId, int cantidad, string tipoOperacion);
}
