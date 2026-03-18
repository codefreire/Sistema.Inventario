using Sistema.Inventario.Transaccion.Infraestructura.Persistencia;

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
}