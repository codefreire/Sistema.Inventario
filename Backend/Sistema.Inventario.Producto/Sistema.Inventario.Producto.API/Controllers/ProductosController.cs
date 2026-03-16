using Microsoft.AspNetCore.Mvc;
using Sistema.Inventario.Producto.Aplicacion.DTOs.Requests;
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
    /// Handler para crear un Producto
    /// </summary>
    private readonly CrearProductoHandler _crearProductoHandler;

    /// <summary>
    /// Constructor del controlador de Productos
    /// </summary>
    /// <param name="obtenerProductosHandler">Handler para obtener la lista de Productos</param>
    /// <param name="obtenerProductoHandler">Handler para obtener un Producto por su Id</param>
    /// <param name="crearProductoHandler">Handler para crear un Producto</param>
    public ProductosController(ObtenerProductosHandler obtenerProductosHandler, ObtenerProductoHandler obtenerProductoHandler, CrearProductoHandler crearProductoHandler)
    {
        _obtenerProductosHandler = obtenerProductosHandler;
        _obtenerProductoHandler = obtenerProductoHandler;
        _crearProductoHandler = crearProductoHandler;
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
    /// <param name="request">Dato para obtener un Producto por su Id</param>
    /// <returns>Producto encontrado</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductoResponse>> ObtenerProducto([FromRoute] ObtenerProductoPorIdRequest request)
    {
        ProductoResponse? producto = await _obtenerProductoHandler.Handle(request.Id);
        if (producto is null)
        {
            return NotFound($"No se encontró el producto con Id {request.Id}.");
        }
        return Ok(producto);
    }

    /// <summary>
    /// Endpoint para crear un Producto
    /// </summary>
    /// <param name="request">Datos del Producto a crear</param>
    /// <returns>Producto creado</returns>
    [HttpPost]
    public async Task<ActionResult<ProductoResponse>> CrearProducto([FromBody] CrearProductoRequest request)
    {
        ProductoResponse producto = await _crearProductoHandler.Handle(request);
        return CreatedAtAction(nameof(ObtenerProducto), new { id = producto.Id }, producto);
    }
}