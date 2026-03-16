using Microsoft.AspNetCore.Mvc;
using Sistema.Inventario.Producto.Aplicacion.DTOs.Responses;
using Sistema.Inventario.Producto.Aplicacion.Handlers;

namespace Sistema.Inventario.Producto.API.Controllers;

/// <summary>
/// Controlador para manejar las operaciones de los Productos
/// </summary>
[ApiController]
[Route("api/productos")]
public class ProductosController : ControllerBase
{
    /// <summary>
    /// Handler para obtener la lista de Productos
    /// </summary>
    private readonly ObtenerProductosHandler _obtenerProductosHandler;

    /// <summary>
    /// Constructor del controlador de Productos
    /// </summary>
    /// <param name="obtenerProductosHandler">Handler para obtener la lista de Productos</param>
    public ProductosController(ObtenerProductosHandler obtenerProductosHandler)
    {
        _obtenerProductosHandler = obtenerProductosHandler;
    }

    /// <summary>
    /// Endpoint para obtener la lista de Productos
    /// </summary>
    /// <returns>Lista de productos</returns>
    [HttpGet]
    public async Task<ActionResult<List<ProductoResponse>>> ObtenerProductos()
    {
        List<ProductoResponse> productos = await _obtenerProductosHandler.Handle();
        return Ok(productos);
    }
}