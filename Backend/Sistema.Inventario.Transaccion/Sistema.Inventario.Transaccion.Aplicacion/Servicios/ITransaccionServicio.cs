namespace Sistema.Inventario.Transaccion.Aplicacion.Servicios;

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
}