namespace Sistema.Inventario.Producto.Aplicacion.DTOs.Requests;

/// <summary>
/// Clase que representa la solicitud para obtener un Producto por su Id
/// </summary>
public class ObtenerProductoPorIdRequest
{
    /// <summary>
    /// Identificador del Producto a obtener
    /// </summary>
    public Guid Id { get; set; }
}