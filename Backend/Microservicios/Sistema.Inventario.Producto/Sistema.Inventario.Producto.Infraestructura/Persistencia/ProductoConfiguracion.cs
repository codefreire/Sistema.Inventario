using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sistema.Inventario.Producto.Dominio.Entidades;

namespace Sistema.Inventario.Producto.Infraestructura.Persistencia;

/// <summary>
/// Clase de configuración que define la estructura y las restricciones de los campos de la entidad Producto
/// </summary>
public class ProductoConfiguracion : IEntityTypeConfiguration<ProductoEntidad>
{
    /// <summary>
    /// Método para configurar la entidad Producto
    /// </summary>
    /// <param name="builder">Constructor para configurar la entidad Producto</param>
    public void Configure(EntityTypeBuilder<ProductoEntidad> builder)
    {
        builder.ToTable("Productos", table =>
        {
            table.HasCheckConstraint("CK_Productos_Precio", "Precio >= 0");
            table.HasCheckConstraint("CK_Productos_Stock", "Stock >= 0");
        });

        builder.HasKey(producto => producto.Id);

        builder.Property(producto => producto.Id)
            .HasDefaultValueSql("NEWID()");

        builder.Property(producto => producto.Nombre)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(producto => producto.Descripcion)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(producto => producto.Categoria)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(producto => producto.ImagenUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(producto => producto.Precio)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(producto => producto.Stock)
            .IsRequired();
    }
}