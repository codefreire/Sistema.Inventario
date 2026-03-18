using Microsoft.AspNetCore.Mvc;
using Sistema.Inventario.Transaccion.Aplicacion.DTOs.Responses;
using Sistema.Inventario.Transaccion.Aplicacion.Handlers;
namespace Sistema.Inventario.Transaccion.API.Controllers;

/// <summary>
/// Controlador para manejar las operaciones de las Transacciones
/// </summary>
[ApiController]
[Route("api/transacciones")]
public class TransaccionesController : ControllerBase
{
    /// <summary>
    /// Handler para obtener la lista de Transacciones
    /// </summary>
    private readonly ObtenerTransaccionesHandler _obtenerTransaccionesHandler;

    /// <summary>
    /// Constructor del controlador de Transacciones
    /// </summary>
    /// <param name="obtenerTransaccionesHandler">Handler para obtener la lista de Transacciones</param>
    public TransaccionesController(ObtenerTransaccionesHandler obtenerTransaccionesHandler)
    {
        _obtenerTransaccionesHandler = obtenerTransaccionesHandler;
    }

    /// <summary>
    /// Endpoint para obtener la lista de Transacciones
    /// </summary>
    /// <returns>Lista de transacciones</returns>
    [HttpGet]
    public async Task<ActionResult<List<TransaccionResponse>>> ObtenerTransferencias()
    {
        List<TransaccionResponse> transacciones = await _obtenerTransaccionesHandler.Handle();
        return Ok(transacciones);
    }
}