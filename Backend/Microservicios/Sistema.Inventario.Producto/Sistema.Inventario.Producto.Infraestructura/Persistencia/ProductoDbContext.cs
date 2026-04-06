using Microsoft.EntityFrameworkCore;

using Sistema.Inventario.Producto.Dominio.Entidades;

namespace Sistema.Inventario.Producto.Infraestructura.Persistencia;

/// <summary>
/// Clase de contexto de la base de datos para acceder a los datos de Productos en la base de datos
/// </summary>
/// <param name="opciones">Opciones de configuración del contexto de la base de datos</param>
public class ProductoDbContext(DbContextOptions<ProductoDbContext> opciones) : DbContext(opciones)
{
    /// <summary>
    /// DbSet para acceder a los datos de Productos en la base de datos
    /// </summary>
    public DbSet<ProductoEntidad> Productos { get; set; }

    /// <summary>
    /// Método para configurar el modelo de la base de datos para los Productos
    /// </summary>
    /// <param name="modelBuilder">Constructor de modelos para configurar las entidades</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new ProductoConfiguracion());
    }
}

