using Microsoft.EntityFrameworkCore;

using Sistema.Inventario.Transaccion.Dominio.Entidades;

namespace Sistema.Inventario.Transaccion.Infraestructura.Persistencia;

/// <summary>
/// Clase de contexto de la base de datos para acceder a los datos de Transacciones en la base de datos
/// </summary>
/// <param name="opciones">Opciones de configuración del contexto de la base de datos</param>
public class TransaccionDbContext(DbContextOptions<TransaccionDbContext> opciones) : DbContext(opciones)
{
    /// <summary>
    /// DbSet para acceder a los datos de Transacciones en la base de datos
    /// </summary>
    public DbSet<TransaccionEntidad> Transacciones { get; set; }

    /// <summary>
    /// Método para configurar el modelo de la base de datos para las Transacciones
    /// </summary>
    /// <param name="modelBuilder">Constructor de modelos para configurar las entidades</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new TransaccionConfiguracion());
    }
}