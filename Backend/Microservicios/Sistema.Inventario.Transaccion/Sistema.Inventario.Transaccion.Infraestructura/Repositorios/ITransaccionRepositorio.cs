namespace Sistema.Inventario.Transaccion.Infraestructura.Repositorios;

using Sistema.Inventario.Transaccion.Dominio.Entidades;

/// <summary>
/// Interface de repositorio para acceder a los datos de Transacciones en la base de datos
/// </summary>
public interface ITransaccionRepositorio
{
    /// <summary>
    /// Método para obtener la lista de Transacciones
    /// </summary>
    /// <returns>Lista de transacciones</returns>
    Task<List<TransaccionEntidad>> ObtenerTransaccionesAsync();

    /// <summary>
    /// Método para obtener una Transacción por su Id
    /// </summary>
    /// <param name="id">Identificador de la Transacción</param>
    /// <returns>Transacción encontrada o null si no existe</returns>
    Task<TransaccionEntidad?> ObtenerTransaccionPorIdAsync(Guid id);

    /// <summary>
    /// Método para crear una Transacción
    /// </summary>
    /// <param name="transaccionEntidad">Entidad de la Transacción a crear</param>
    Task CrearTransaccionAsync(TransaccionEntidad transaccionEntidad);

    /// <summary>
    /// Método para actualizar una Transacción
    /// </summary>
    /// <param name="id">Identificador de la Transacción</param>
    /// <param name="transaccionEntidad">Datos de la Transacción a actualizar</param>
    /// <returns>Transacción actualizada o null si no existe</returns>
    Task<TransaccionEntidad?> ActualizarTransaccionAsync(Guid id, TransaccionEntidad transaccionEntidad);

    /// <summary>
    /// Método para eliminar una Transacción
    /// </summary>
    /// <param name="id">Identificador de la Transacción</param>
    /// <returns>True si fue eliminada, false si no existe</returns>
    Task<bool> EliminarTransaccionAsync(Guid id);
}