using Sistema.Inventario.Transaccion.Aplicacion.Servicios;

namespace Sistema.Inventario.Transaccion.Aplicacion.Handlers;

/// <summary>
/// Handler para eliminar una Transacción
/// </summary>
public class EliminarTransaccionHandler
{
    /// <summary>
    /// Servicio para manejar la lógica de negocio de Transacciones
    /// </summary>
    private readonly ITransaccionServicio _transaccionServicio;

    /// <summary>
    /// Constructor del handler para eliminar una Transacción
    /// </summary>
    /// <param name="transaccionServicio">Servicio para manejar la lógica de negocio de Transacciones</param>
    public EliminarTransaccionHandler(ITransaccionServicio transaccionServicio)
    {
        _transaccionServicio = transaccionServicio;
    }

    /// <summary>
    /// Método para manejar la eliminación de una Transacción
    /// </summary>
    /// <param name="id">Identificador de la Transacción</param>
    /// <returns>True si fue eliminada, false si no existe</returns>
    public async Task<bool> Handle(Guid id)
    {
        return await _transaccionServicio.EliminarTransaccionAsync(id);
    }
}