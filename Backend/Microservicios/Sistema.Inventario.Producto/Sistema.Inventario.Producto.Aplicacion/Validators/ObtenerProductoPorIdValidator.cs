using FluentValidation;

using Sistema.Inventario.Producto.Aplicacion.DTOs.Requests;

namespace Sistema.Inventario.Producto.Aplicacion.Validators;

/// <summary>
/// Validator para la solicitud de obtener un Producto por su Id
/// </summary>
public class ObtenerProductoPorIdValidator : AbstractValidator<ObtenerProductoPorIdRequest>
{
    /// <summary>
    /// Constructor del validator para la solicitud de obtener un Producto por su Id
    /// </summary>
    public ObtenerProductoPorIdValidator()
    {
        RuleFor(request => request.Id)
            .NotEmpty()
            .WithMessage("El Id del producto es obligatorio.");
    }
}