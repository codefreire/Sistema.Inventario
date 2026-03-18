using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sistema.Inventario.Transaccion.Dominio.Entidades;

namespace Sistema.Inventario.Transaccion.Infraestructura.Persistencia;

/// <summary>
/// Clase de configuración que define la estructura y las restricciones de los campos de la entidad Transacción
/// </summary>
public class TransaccionConfiguracion : IEntityTypeConfiguration<TransaccionEntidad>
{
    /// <summary>
    /// Método para configurar la entidad Transacción
    /// </summary>
    /// <param name="builder">Constructor para configurar la entidad Transacción</param>
    public void Configure(EntityTypeBuilder<TransaccionEntidad> builder)
    {

    }
}