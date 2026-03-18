namespace Sistema.Inventario.Transaccion.Aplicacion.Servicios;

using Sistema.Inventario.Transaccion.Aplicacion.DTOs.Requests;
using Sistema.Inventario.Transaccion.Aplicacion.DTOs.Responses;

/// <summary>
/// Interface de servicio para manejar la lógica de negocio de Transacciones
/// </summary>
public interface ITransaccionServicio
{
    /// <summary>
    /// Método para obtener la lista de Transacciones
    /// </summary>
    /// <returns>Lista de transacciones</returns>
    Task<List<TransaccionResponse>> ObtenerTransaccionesAsync();

    /// <summary>
    /// Método para obtener una Transacción por su Id
    /// </summary>
    /// <param name="id">Identificador de la Transacción</param>
    /// <returns>Transacción encontrada o null si no existe</returns>
    Task<TransaccionResponse?> ObtenerTransaccionPorIdAsync(Guid id);

    /// <summary>
    /// Método para crear una Transacción
    /// </summary>
    /// <param name="request">Datos de la Transacción a crear</param>
    /// <returns>Transacción creada</returns>
    Task<TransaccionResponse> CrearTransaccionAsync(CrearTransaccionRequest request);
}