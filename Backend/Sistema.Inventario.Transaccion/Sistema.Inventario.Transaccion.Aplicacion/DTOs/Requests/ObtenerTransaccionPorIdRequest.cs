namespace Sistema.Inventario.Transaccion.Aplicacion.DTOs.Requests;

/// <summary>
/// Clase que representa la solicitud para obtener una Transacción por su Id
/// </summary>
public class ObtenerTransaccionPorIdRequest
{
    /// <summary>
    /// Identificador de la Transacción a obtener
    /// </summary>
    public Guid Id { get; set; }
}