using System.ComponentModel.DataAnnotations;

namespace Sistema.Inventario.Producto.Dominio.Entidades;

/// <summary>
/// Clase que representa la entidad de un Producto en la base de datos
/// </summary>
public class ProductoEntidad
{
    /// <summary>
    /// Identificador único del Producto
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Nombre del Producto
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Nombre { get; set; }

    /// <summary>
    /// Descripción del Producto
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string Descripcion { get; set; }

    /// <summary>
    /// Categoría del Producto
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Categoria { get; set; }

    /// <summary>
    /// Url de la imagen del Producto
    /// </summary>
    [Url]
    [Required]
    [MaxLength(500)]
    public string ImagenUrl { get; set; }

    /// <summary>
    /// El precio del Producto
    /// </summary>
    [Range(typeof(decimal), "0", "99999999.99", ErrorMessage = "El precio debe ser mayor o igual a 0.")]
    public decimal Precio { get; set; }

    /// <summary>
    /// Cantidad de Productos en stock
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser mayor o igual a 0.")]
    public int Stock { get; set; }
}