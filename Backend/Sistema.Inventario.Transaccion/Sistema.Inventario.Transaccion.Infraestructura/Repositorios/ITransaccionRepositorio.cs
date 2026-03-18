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
}