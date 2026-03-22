using Sistema.Inventario.Transaccion.Aplicacion.DTOs.Responses;
using Sistema.Inventario.Transaccion.Aplicacion.Servicios;

namespace Sistema.Inventario.Transaccion.Aplicacion.Handlers;

/// <summary>
/// Handler para obtener la lista de Transacciones
/// </summary>
public class ObtenerTransaccionesHandler
{
    /// <summary>
    /// Servicio para manejar la lógica de negocio de Transacciones
    /// </summary>
    private readonly ITransaccionServicio _transaccionServicio;

    /// <summary>
    /// Constructor del handler para obtener la lista de Transacciones
    /// </summary>
    /// <param name="transaccionServicio">Servicio para manejar la lógica de negocio de Transacciones</param>
    public ObtenerTransaccionesHandler(ITransaccionServicio transaccionServicio)
    {
        _transaccionServicio = transaccionServicio;
    }

    /// <summary>
    /// Método para manejar la obtención de la lista de Transacciones
    /// </summary>
    /// <returns>Lista de transacciones</returns>
    public async Task<List<TransaccionResponse>> Handle()
    {
        return await _transaccionServicio.ObtenerTransaccionesAsync();
    }
}