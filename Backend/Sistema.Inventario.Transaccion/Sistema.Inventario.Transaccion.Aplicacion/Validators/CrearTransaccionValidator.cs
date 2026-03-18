using FluentValidation;
using Sistema.Inventario.Transaccion.Aplicacion.DTOs.Requests;

namespace Sistema.Inventario.Transaccion.Aplicacion.Validators;

/// <summary>
/// Validator para la solicitud de crear una Transacción
/// </summary>
public class CrearTransaccionValidator : AbstractValidator<CrearTransaccionRequest>
{
    /// <summary>
    /// Constructor del validator para la solicitud de crear una Transacción
    /// </summary>
    public CrearTransaccionValidator()
    {

    }
}