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
}