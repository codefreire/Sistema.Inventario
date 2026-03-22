using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema.Inventario.Transaccion.Dominio.Entidades;

/// <summary>
/// Clase que representa la entidad de una Transacción en la base de datos
/// </summary>
public class TransaccionEntidad
{
    /// <summary>
    /// Identificador único de la Transacción
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Fecha de la Transacción
    /// </summary>
    [Required]
    public DateTime Fecha { get; set; }

    /// <summary>
    /// Tipo de la Transacción (Compra o Venta)
    /// </summary>
    [Required]
    [MaxLength(10)]
    [RegularExpression("(?i)^(compra|venta)$", ErrorMessage = "El tipo de transacción debe ser Compra o Venta.")]
    public string TipoTransaccion { get; set; }

    /// <summary>
    /// Identificador del Producto asociado a la Transacción
    /// </summary>
    [Required]
    public Guid ProductoId { get; set; }

    /// <summary>
    /// Cantidad de Productos de la Transacción
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
    public int Cantidad { get; set; }

    /// <summary>
    /// Precio unitario del Producto en la Transacción
    /// </summary>
    [Required]
    [Range(typeof(decimal), "0", "99999999.99", ErrorMessage = "El precio unitario debe ser mayor o igual a 0.")]
    public decimal PrecioUnitario { get; set; }

    /// <summary>
    /// Precio total del Producto en la Transacción (Cantidad * PrecioUnitario)
    /// </summary>
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [Range(typeof(decimal), "0", "9999999999.99", ErrorMessage = "El precio total debe ser mayor o igual a 0.")]
    public decimal PrecioTotal { get; set; }

    /// <summary>
    /// Detalle de la Transacción
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string Detalle { get; set; }
}