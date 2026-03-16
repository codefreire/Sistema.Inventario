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
    /// Handler para obtener un Producto por su Id
    /// </summary>
    private readonly ObtenerProductoHandler _obtenerProductoHandler;

    /// <summary>
    /// Constructor del controlador de Productos
    /// </summary>
    /// <param name="obtenerProductosHandler">Handler para obtener la lista de Productos</param>
    /// <param name="obtenerProductoHandler">Handler para obtener un Producto por su Id</param>
    public ProductosController(ObtenerProductosHandler obtenerProductosHandler, ObtenerProductoHandler obtenerProductoHandler)
    {
        _obtenerProductosHandler = obtenerProductosHandler;
        _obtenerProductoHandler = obtenerProductoHandler;
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

    /// <summary>
    /// Endpoint para obtener un Producto por su Id
    /// </summary>
    /// <param name="id">Identificador del Producto</param>
    /// <returns>Producto encontrado</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductoResponse>> ObtenerProducto(Guid id)
    {
        ProductoResponse? producto = await _obtenerProductoHandler.Handle(id);
        if (producto is null)
        {
            return NotFound($"No se encontró el producto con Id {id}.");
        }
        return Ok(producto);
    }
}