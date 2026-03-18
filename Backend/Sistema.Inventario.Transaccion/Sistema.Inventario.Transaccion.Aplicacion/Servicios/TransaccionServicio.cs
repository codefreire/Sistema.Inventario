using Sistema.Inventario.Transaccion.Aplicacion.DTOs.Responses;
using Sistema.Inventario.Transaccion.Dominio.Entidades;
using Sistema.Inventario.Transaccion.Infraestructura.Repositorios;

namespace Sistema.Inventario.Transaccion.Aplicacion.Servicios;

/// <summary>
/// Clase de servicio para manejar la lógica de negocio de Transacciones
/// </summary>
public class TransaccionServicio : ITransaccionServicio
{
    /// <summary>
    /// Repositorio para acceder a los datos de Transacciones
    /// </summary>
    private readonly ITransaccionRepositorio _transaccionRepositorio;

    /// <summary>
    /// Constructor del servicio para manejar la lógica de negocio de Transacciones
    /// </summary>
    /// <param name="transaccionRepositorio">Repositorio para acceder a los datos de Transacciones</param>
    public TransaccionServicio(ITransaccionRepositorio transaccionRepositorio)
    {
        _transaccionRepositorio = transaccionRepositorio;
    }

    /// <summary>
    /// Método para obtener la lista de Transacciones
    /// </summary>
    /// <returns>Lista de transacciones</returns>
    public async Task<List<TransaccionResponse>> ObtenerTransaccionesAsync()
    {
        List<TransaccionEntidad> transacciones = await _transaccionRepositorio.ObtenerTransaccionesAsync();
        return transacciones.Select(transaccion => new TransaccionResponse
        {
            Id = transaccion.Id,
            Fecha = transaccion.Fecha,
            TipoTransaccion = transaccion.TipoTransaccion,
            ProductoId = transaccion.ProductoId,
            Cantidad = transaccion.Cantidad,
            PrecioUnitario = transaccion.PrecioUnitario,
            PrecioTotal = transaccion.PrecioTotal,
            Detalle = transaccion.Detalle
        }).ToList();
    }

    /// <summary>
    /// Método para obtener una Transacción por su Id
    /// </summary>
    /// <param name="id">Identificador de la Transacción</param>
    /// <returns>Transacción encontrada o null si no existe</returns>
    public async Task<TransaccionResponse?> ObtenerTransaccionPorIdAsync(Guid id)
    {
        TransaccionEntidad? transaccion = await _transaccionRepositorio.ObtenerTransaccionPorIdAsync(id);
        if (transaccion is null)
        {
            return null;
        }
        return new TransaccionResponse
        {
            Id = transaccion.Id,
            Fecha = transaccion.Fecha,
            TipoTransaccion = transaccion.TipoTransaccion,
            ProductoId = transaccion.ProductoId,
            Cantidad = transaccion.Cantidad,
            PrecioUnitario = transaccion.PrecioUnitario,
            PrecioTotal = transaccion.PrecioTotal,
            Detalle = transaccion.Detalle
        };
    }
}