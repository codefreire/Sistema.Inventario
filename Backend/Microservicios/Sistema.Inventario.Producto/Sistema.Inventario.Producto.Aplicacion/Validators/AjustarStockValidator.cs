using FluentValidation;
using Sistema.Inventario.Producto.Aplicacion.DTOs.Requests;

namespace Sistema.Inventario.Producto.Aplicacion.Validators;

/// <summary>
/// Validador para la solicitud de ajuste de stock de un Producto
/// </summary>
public class AjustarStockValidator : AbstractValidator<AjustarStockRequest>
{
    /// <summary>
    /// Constructor del validador de ajuste de stock
    /// </summary>
    public AjustarStockValidator()
    {
        RuleFor(request => request.Cantidad)
            .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0.");

        RuleFor(request => request.TipoOperacion)
            .NotEmpty().WithMessage("El tipo de operación es requerido.")
            .Matches("(?i)^(compra|venta)$").WithMessage("El tipo de operación debe ser Compra o Venta.");
    }
}
