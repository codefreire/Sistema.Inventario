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
        builder.ToTable("Transacciones", tabla =>
        {
            tabla.HasCheckConstraint("CK_Transacciones_TipoTransaccion", "[TipoTransaccion] IN (N'Compra', N'Venta')");
            tabla.HasCheckConstraint("CK_Transacciones_Cantidad", "[Cantidad] > 0");
            tabla.HasCheckConstraint("CK_Transacciones_PrecioUnitario", "[PrecioUnitario] >= 0");
        });

        builder.HasKey(transaccion => transaccion.Id);

        builder.Property(transaccion => transaccion.Id)
            .HasColumnName("Id")
            .HasDefaultValueSql("NEWID()");

        builder.Property(transaccion => transaccion.Fecha)
            .HasColumnName("Fecha")
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        builder.Property(transaccion => transaccion.TipoTransaccion)
            .HasColumnName("TipoTransaccion")
            .HasMaxLength(10)
            .IsUnicode()
            .IsRequired();

        builder.Property(transaccion => transaccion.ProductoId)
            .HasColumnName("ProductoId")
            .IsRequired();

        builder.Property(transaccion => transaccion.Cantidad)
            .HasColumnName("Cantidad")
            .IsRequired();

        builder.Property(transaccion => transaccion.PrecioUnitario)
            .HasColumnName("PrecioUnitario")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(transaccion => transaccion.PrecioTotal)
            .HasColumnName("PrecioTotal")
            .HasColumnType("decimal(10,2)")
            .HasComputedColumnSql("CAST((Cantidad * PrecioUnitario) AS DECIMAL(10,2))", stored: true);

        builder.Property(transaccion => transaccion.Detalle)
            .HasColumnName("Detalle")
            .HasMaxLength(500)
            .IsUnicode()
            .IsRequired();
    }
}