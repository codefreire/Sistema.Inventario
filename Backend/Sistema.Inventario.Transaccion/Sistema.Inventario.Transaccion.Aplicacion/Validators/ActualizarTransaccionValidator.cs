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

    }
}