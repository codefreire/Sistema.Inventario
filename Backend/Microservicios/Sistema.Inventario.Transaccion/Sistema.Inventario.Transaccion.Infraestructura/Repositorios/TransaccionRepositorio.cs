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
        return await _contexto.Transacciones
            .AsNoTracking()
            .OrderByDescending(transaccion => transaccion.Fecha)
            .ThenByDescending(transaccion => transaccion.Id)
            .ToListAsync();
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

    /// <summary>
    /// Método para crear una Transacción
    /// </summary>
    /// <param name="transaccionEntidad">Entidad de la Transacción a crear</param>
    public async Task CrearTransaccionAsync(TransaccionEntidad transaccionEntidad)
    {
        await _contexto.Transacciones.AddAsync(transaccionEntidad);
        await _contexto.SaveChangesAsync();
    }

    /// <summary>
    /// Método para actualizar una Transacción
    /// </summary>
    /// <param name="id">Identificador de la Transacción</param>
    /// <param name="transaccionEntidad">Datos de la Transacción a actualizar</param>
    /// <returns>Transacción actualizada o null si no existe</returns>
    public async Task<TransaccionEntidad?> ActualizarTransaccionAsync(Guid id, TransaccionEntidad transaccionEntidad)
    {
        TransaccionEntidad? transaccion = await _contexto.Transacciones.FirstOrDefaultAsync(transaccion => transaccion.Id == id);
        if (transaccion is null)
        {
            return null;
        }

        transaccion.TipoTransaccion = transaccionEntidad.TipoTransaccion;
        transaccion.ProductoId = transaccionEntidad.ProductoId;
        transaccion.Cantidad = transaccionEntidad.Cantidad;
        transaccion.PrecioUnitario = transaccionEntidad.PrecioUnitario;
        transaccion.PrecioTotal = transaccionEntidad.PrecioTotal;
        transaccion.Detalle = transaccionEntidad.Detalle;

        await _contexto.SaveChangesAsync();
        return transaccion;
    }

    /// <summary>
    /// Método para eliminar una Transacción
    /// </summary>
    /// <param name="id">Identificador de la Transacción</param>
    /// <returns>True si fue eliminada, false si no existe</returns>
    public async Task<bool> EliminarTransaccionAsync(Guid id)
    {
        TransaccionEntidad? transaccion = await _contexto.Transacciones.FirstOrDefaultAsync(transaccion => transaccion.Id == id);
        if (transaccion is null)
        {
            return false;
        }

        _contexto.Transacciones.Remove(transaccion);
        await _contexto.SaveChangesAsync();
        return true;
    }
}