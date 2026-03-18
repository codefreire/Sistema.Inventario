using Microsoft.AspNetCore.Mvc;
using Sistema.Inventario.Transaccion.Aplicacion.DTOs.Requests;
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
    /// Handler para obtener una Transacción por su Id
    /// </summary>
    private readonly ObtenerTransaccionHandler _obtenerTransaccionHandler;

    /// <summary>
    /// Handler para crear una Transacción
    /// </summary>
    private readonly CrearTransaccionHandler _crearTransaccionHandler;

    /// <summary>
    /// Constructor del controlador de Transacciones
    /// </summary>
    /// <param name="obtenerTransaccionesHandler">Handler para obtener la lista de Transacciones</param>
    /// <param name="obtenerTransaccionHandler">Handler para obtener una Transacción por su Id</param>
    /// <param name="crearTransaccionHandler">Handler para crear una Transacción</param>
    public TransaccionesController(ObtenerTransaccionesHandler obtenerTransaccionesHandler, ObtenerTransaccionHandler obtenerTransaccionHandler, CrearTransaccionHandler crearTransaccionHandler)
    {
        _obtenerTransaccionesHandler = obtenerTransaccionesHandler;
        _obtenerTransaccionHandler = obtenerTransaccionHandler;
        _crearTransaccionHandler = crearTransaccionHandler;
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

    /// <summary>
    /// Endpoint para obtener una Transacción por su Id
    /// </summary>
    /// <param name="request">Dato para obtener una Transacción por su Id</param>
    /// <returns>Transacción encontrada</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TransaccionResponse>> ObtenerTransferencia([FromRoute] ObtenerTransaccionPorIdRequest request)
    {
        TransaccionResponse? transaccion = await _obtenerTransaccionHandler.Handle(request.Id);
        if (transaccion is null)
        {
            return NotFound($"No se encontró la transacción con Id {request.Id}.");
        }
        return Ok(transaccion);
    }

    /// <summary>
    /// Endpoint para crear una Transacción
    /// </summary>
    /// <param name="request">Datos de la Transacción a crear</param>
    /// <returns>Transacción creada</returns>
    [HttpPost]
    public async Task<ActionResult<TransaccionResponse>> CrearTransaccion([FromBody] CrearTransaccionRequest request)
    {
        TransaccionResponse transaccion = await _crearTransaccionHandler.Handle(request);
        return CreatedAtAction(nameof(ObtenerTransferencia), new { id = transaccion.Id }, transaccion);
    }
}