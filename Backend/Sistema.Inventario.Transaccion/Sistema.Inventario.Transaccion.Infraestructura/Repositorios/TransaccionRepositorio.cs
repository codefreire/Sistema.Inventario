using Microsoft.EntityFrameworkCore;
using Sistema.Inventario.Transaccion.Infraestructura.Persistencia;
using Sistema.Inventario.Transaccion.Dominio.Entidades;

namespace Sistema.Inventario.Transaccion.Infraestructura.Repositorios;

/// <summary>
/// Clase de repositorio para acceder a los datos de Transacciones en la base de datos
/// </summary>
public class TransaccionRepositorio : ITransaccionRepositorio
{
    /// <summary>
    /// Contexto de la base de datos para acceder a los datos de Transacciones
    /// </summary>
    private readonly TransaccionDbContext _contexto;

    /// <summary>
    /// Constructor del repositorio para acceder a los datos de Transacciones en la base de datos
    /// </summary>
    /// <param name="contexto">Contexto de la base de datos</param>
    public TransaccionRepositorio(TransaccionDbContext contexto)
    {
        _contexto = contexto;
    }

    /// <summary>
    /// Método para obtener la lista de Transacciones
    /// </summary>
    /// <returns>Lista de transacciones</returns>
    public async Task<List<TransaccionEntidad>> ObtenerTransaccionesAsync()
    {
        return await _contexto.Transacciones.AsNoTracking().ToListAsync();
    }

    /// <summary>
    /// Método para obtener una Transacción por su Id
    /// </summary>
    /// <param name="id">Identificador de la Transacción</param>
    /// <returns>Transacción encontrada o null si no existe</returns>
    public async Task<TransaccionEntidad?> ObtenerTransaccionPorIdAsync(Guid id)
    {
        return await _contexto.Transacciones.AsNoTracking().FirstOrDefaultAsync(transaccion => transaccion.Id == id);
    }
}