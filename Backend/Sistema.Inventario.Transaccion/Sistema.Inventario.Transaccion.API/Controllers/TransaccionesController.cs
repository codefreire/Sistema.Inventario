using Microsoft.AspNetCore.Mvc;
namespace Sistema.Inventario.Transaccion.API.Controllers;

/// <summary>
/// Controlador para manejar las operaciones de las Transacciones
/// </summary>
[ApiController]
[Route("api/transacciones")]
public class TransaccionesController : ControllerBase
{
    /// <summary>
    /// Constructor del controlador de Transacciones
    /// </summary>
    public TransaccionesController()
    {
    }
}