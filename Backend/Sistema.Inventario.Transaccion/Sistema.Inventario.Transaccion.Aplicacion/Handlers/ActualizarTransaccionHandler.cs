using Sistema.Inventario.Transaccion.Aplicacion.DTOs.Requests;
using Sistema.Inventario.Transaccion.Aplicacion.DTOs.Responses;
using Sistema.Inventario.Transaccion.Aplicacion.Servicios;

namespace Sistema.Inventario.Transaccion.Aplicacion.Handlers;

/// <summary>
/// Handler para actualizar una Transacción
/// </summary>
public class ActualizarTransaccionHandler
{
    /// <summary>
    /// Servicio para manejar la lógica de negocio de Transacciones
    /// </summary>
    private readonly ITransaccionServicio _transaccionServicio;

    /// <summary>
    /// Constructor del handler para actualizar una Transacción
    /// </summary>
    /// <param name="transaccionServicio">Servicio para manejar la lógica de negocio de Transacciones</param>
    public ActualizarTransaccionHandler(ITransaccionServicio transaccionServicio)
    {
        _transaccionServicio = transaccionServicio;
    }

    /// <summary>
    /// Método para manejar la actualización de una Transacción
    /// </summary>
    /// <param name="id">Identificador de la Transacción</param>
    /// <param name="request">Datos de la Transacción a actualizar</param>
    /// <returns>Transacción actualizada o null si no existe</returns>
    public async Task<TransaccionResponse?> Handle(Guid id, ActualizarTransaccionRequest request)
    {
        return await _transaccionServicio.ActualizarTransaccionAsync(id, request);
    }
}