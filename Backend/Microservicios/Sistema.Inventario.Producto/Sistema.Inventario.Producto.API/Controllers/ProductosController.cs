using Microsoft.AspNetCore.Mvc;

using Sistema.Inventario.Producto.Aplicacion.DTOs.Requests;
using Sistema.Inventario.Producto.Aplicacion.DTOs.Responses;
using Sistema.Inventario.Producto.Aplicacion.Handlers;

namespace Sistema.Inventario.Producto.API.Controllers;

/// <summary>
/// Controlador para manejar las operaciones de los Productos
/// </summary>
[ApiController]
[Route("api/[controller]")]
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
    /// Handler para actualizar un Producto
    /// </summary>
    private readonly ActualizarProductoHandler _actualizarProductoHandler;

    /// <summary>
    /// Handler para eliminar un Producto
    /// </summary>
    private readonly EliminarProductoHandler _eliminarProductoHandler;

    /// <summary>
    /// Handler para ajustar el stock de un Producto
    /// </summary>
    private readonly AjustarStockHandler _ajustarStockHandler;

    /// <summary>
    /// Handler para subir imágenes de Producto
    /// </summary>
    private readonly SubirImagenProductoHandler _subirImagenHandler;

    /// <summary>
    /// Constructor del controlador de Productos
    /// </summary>
    /// <param name="obtenerProductosHandler">Handler para obtener la lista de Productos</param>
    /// <param name="obtenerProductoHandler">Handler para obtener un Producto por su Id</param>
    /// <param name="crearProductoHandler">Handler para crear un Producto</param>
    /// <param name="actualizarProductoHandler">Handler para actualizar un Producto</param>
    /// <param name="eliminarProductoHandler">Handler para eliminar un Producto</param>
    /// <param name="ajustarStockHandler">Handler para ajustar el stock de un Producto</param>
    /// <param name="subirImagenHandler">Handler para subir imágenes de Producto</param>
    public ProductosController(ObtenerProductosHandler obtenerProductosHandler, ObtenerProductoHandler obtenerProductoHandler, CrearProductoHandler crearProductoHandler, ActualizarProductoHandler actualizarProductoHandler, EliminarProductoHandler eliminarProductoHandler, AjustarStockHandler ajustarStockHandler, SubirImagenProductoHandler subirImagenHandler)
    {
        _obtenerProductosHandler = obtenerProductosHandler;
        _obtenerProductoHandler = obtenerProductoHandler;
        _crearProductoHandler = crearProductoHandler;
        _actualizarProductoHandler = actualizarProductoHandler;
        _eliminarProductoHandler = eliminarProductoHandler;
        _ajustarStockHandler = ajustarStockHandler;
        _subirImagenHandler = subirImagenHandler;
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

    /// <summary>
    /// Endpoint para actualizar un Producto
    /// </summary>
    /// <param name="idRequest">Dato para obtener el Id del Producto</param>
    /// <param name="request">Datos del Producto a actualizar</param>
    /// <returns>Producto actualizado</returns>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductoResponse>> ActualizarProducto([FromRoute] ObtenerProductoPorIdRequest idRequest, [FromBody] ActualizarProductoRequest request)
    {
        ProductoResponse? producto = await _actualizarProductoHandler.Handle(idRequest.Id, request);
        if (producto is null)
        {
            return NotFound($"No se encontró el producto con Id {idRequest.Id}.");
        }
        return Ok(producto);
    }

    /// <summary>
    /// Endpoint para eliminar un Producto
    /// </summary>
    /// <param name="request">Dato para obtener el Id del Producto</param>
    /// <returns>Sin contenido si fue eliminado</returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> EliminarProducto([FromRoute] ObtenerProductoPorIdRequest request)
    {
        bool eliminado = await _eliminarProductoHandler.Handle(request.Id);
        if (!eliminado)
        {
            return NotFound($"No se encontró el producto con Id {request.Id}.");
        }
        return NoContent();
    }

    /// <summary>
    /// Endpoint para ajustar el stock de un Producto
    /// </summary>
    /// <param name="idRequest">Dato para obtener el Id del Producto</param>
    /// <param name="request">Datos del ajuste de stock</param>
    /// <returns>Producto con stock ajustado</returns>
    [HttpPatch("{id:guid}/stock")]
    public async Task<ActionResult<ProductoResponse>> AjustarStock([FromRoute] ObtenerProductoPorIdRequest idRequest, [FromBody] AjustarStockRequest request)
    {
        ProductoResponse? producto = await _ajustarStockHandler.Handle(idRequest.Id, request);
        if (producto is null)
        {
            return BadRequest($"No se pudo ajustar el stock del producto con Id {idRequest.Id}. Verifique que el producto exista y que haya stock suficiente.");
        }
        return Ok(producto);
    }

    /// <summary>
    /// Endpoint para subir una imagen de Producto al almacenamiento
    /// Devuelve la URL que se usa al crear o actualizar un Producto
    /// </summary>
    /// <param name="archivoImagen">Archivo de imagen enviado por formulario multipart</param>
    /// <returns>URL de la imagen almacenada</returns>
    [HttpPost("imagenes")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<SubirImagenResponse>> SubirImagen([FromForm] IFormFile? archivoImagen)
    {
        IFormFile? archivoSubido = archivoImagen ?? Request.Form.Files.FirstOrDefault();
        if (archivoSubido is null)
        {
            return BadRequest("Debe enviar un archivo en el formulario multipart.");
        }

        SubirImagenResponse respuesta = await _subirImagenHandler.Handle(archivoSubido);
        return Ok(respuesta);
    }
}