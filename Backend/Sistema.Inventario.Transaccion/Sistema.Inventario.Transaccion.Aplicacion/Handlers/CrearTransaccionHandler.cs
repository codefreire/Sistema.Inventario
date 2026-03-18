using Sistema.Inventario.Transaccion.Aplicacion.DTOs.Requests;
using Sistema.Inventario.Transaccion.Aplicacion.DTOs.Responses;
using Sistema.Inventario.Transaccion.Aplicacion.Servicios;

namespace Sistema.Inventario.Transaccion.Aplicacion.Handlers;

/// <summary>
/// Handler para crear una Transacción
/// </summary>
public class CrearTransaccionHandler
{
    /// <summary>
    /// Servicio para manejar la lógica de negocio de Transacciones
    /// </summary>
    private readonly ITransaccionServicio _transaccionServicio;

    /// <summary>
    /// Constructor del handler para crear una Transacción
    /// </summary>
    /// <param name="transaccionServicio">Servicio para manejar la lógica de negocio de Transacciones</param>
    public CrearTransaccionHandler(ITransaccionServicio transaccionServicio)
    {
        _transaccionServicio = transaccionServicio;
    }

    /// <summary>
    /// Método para manejar la creación de una Transacción
    /// </summary>
    /// <param name="request">Datos de la Transacción a crear</param>
    /// <returns>Transacción creada</returns>
    public async Task<TransaccionResponse> Handle(CrearTransaccionRequest request)
    {
        return await _transaccionServicio.CrearTransaccionAsync(request);
    }
}