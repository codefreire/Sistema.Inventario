using FluentValidation;
using Sistema.Inventario.Transaccion.Aplicacion.DTOs.Requests;

namespace Sistema.Inventario.Transaccion.Aplicacion.Validators;

/// <summary>
/// Validador para la solicitud de actualizar una Transacción
/// </summary>
public class ActualizarTransaccionValidator : AbstractValidator<ActualizarTransaccionRequest>
{
    /// <summary>
    /// Constructor del validador para la solicitud de actualizar una Transacción
    /// </summary>
    public ActualizarTransaccionValidator()
    {
        RuleFor(request => request.TipoTransaccion)
            .NotEmpty()
            .WithMessage("El tipo de transacción es obligatorio.")
            .MaximumLength(10)
            .WithMessage("El tipo de transacción no puede tener más de 10 caracteres.")
            .Matches("(?i)^(compra|venta)$")
            .WithMessage("El tipo de transacción debe ser Compra o Venta.");

        RuleFor(request => request.ProductoId)
            .NotEmpty()
            .WithMessage("El Id del producto es obligatorio.");

        RuleFor(request => request.Cantidad)
            .GreaterThan(0)
            .WithMessage("La cantidad debe ser mayor a 0.");

        RuleFor(request => request.PrecioUnitario)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El precio unitario debe ser mayor o igual a 0.");

        RuleFor(request => request.Detalle)
            .NotEmpty()
            .WithMessage("El detalle de la transacción es obligatorio.")
            .MaximumLength(500)
            .WithMessage("El detalle de la transacción no puede tener más de 500 caracteres.");
    }
}