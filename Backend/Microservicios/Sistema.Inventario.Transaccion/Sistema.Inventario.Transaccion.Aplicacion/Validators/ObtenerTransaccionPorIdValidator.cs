using FluentValidation;
using Sistema.Inventario.Transaccion.Aplicacion.DTOs.Requests;

namespace Sistema.Inventario.Transaccion.Aplicacion.Validators;

/// <summary>
/// Validator para la solicitud de obtener una Transacción por su Id
/// </summary>
public class ObtenerTransaccionPorIdValidator : AbstractValidator<ObtenerTransaccionPorIdRequest>
{
    /// <summary>
    /// Constructor del validator para la solicitud de obtener una Transacción por su Id
    /// </summary>
    public ObtenerTransaccionPorIdValidator()
    {
        RuleFor(request => request.Id)
            .NotEmpty()
            .WithMessage("El Id de la transacción es obligatorio.");
    }
}