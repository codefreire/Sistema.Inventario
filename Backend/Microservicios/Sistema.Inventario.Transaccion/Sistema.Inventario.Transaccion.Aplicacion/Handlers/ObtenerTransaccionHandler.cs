using Sistema.Inventario.Transaccion.Aplicacion.DTOs.Responses;
using Sistema.Inventario.Transaccion.Aplicacion.Servicios;

namespace Sistema.Inventario.Transaccion.Aplicacion.Handlers;

/// <summary>
/// Handler para obtener una Transacción por su Id
/// </summary>
public class ObtenerTransaccionHandler
{
    /// <summary>
    /// Servicio para manejar la lógica de negocio de Transacciones
    /// </summary>
    private readonly ITransaccionServicio _transaccionServicio;

    /// <summary>
    /// Constructor del handler para obtener una Transacción por su Id
    /// </summary>
    /// <param name="transaccionServicio">Servicio para manejar la lógica de negocio de Transacciones</param>
    public ObtenerTransaccionHandler(ITransaccionServicio transaccionServicio)
    {
        _transaccionServicio = transaccionServicio;
    }

    /// <summary>
    /// Método para manejar la obtención de una Transacción por su Id
    /// </summary>
    /// <param name="id">Identificador de la Transacción</param>
    /// <returns>Transacción encontrada o null si no existe</returns>
    public async Task<TransaccionResponse?> Handle(Guid id)
    {
        return await _transaccionServicio.ObtenerTransaccionPorIdAsync(id);
    }
}