using FluentValidation;

using Sistema.Inventario.Producto.Aplicacion.DTOs.Requests;

namespace Sistema.Inventario.Producto.Aplicacion.Validators;

/// <summary>
/// Validator para la solicitud de actualizar un Producto
/// </summary>
public class ActualizarProductoValidator : AbstractValidator<ActualizarProductoRequest>
{
    /// <summary>
    /// Constructor del validator para la solicitud de actualizar un Producto
    /// </summary>
    public ActualizarProductoValidator()
    {
        RuleFor(request => request.Nombre)
            .NotEmpty()
            .WithMessage("El nombre del producto es obligatorio.")
            .MaximumLength(50)
            .WithMessage("El nombre del producto no puede tener más de 50 caracteres.");

        RuleFor(request => request.Descripcion)
            .NotEmpty()
            .WithMessage("La descripción del producto es obligatoria.")
            .MaximumLength(500)
            .WithMessage("La descripción del producto no puede tener más de 500 caracteres.");

        RuleFor(request => request.Categoria)
            .NotEmpty()
            .WithMessage("La categoría del producto es obligatoria.")
            .MaximumLength(50)
            .WithMessage("La categoría del producto no puede tener más de 50 caracteres.");

        RuleFor(request => request.ImagenUrl)
            .NotEmpty()
            .WithMessage("La Url de la imagen del producto es obligatoria.")
            .MaximumLength(500)
            .WithMessage("La Url de la imagen del producto no puede tener más de 500 caracteres.")
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("La Url de la imagen del producto no tiene un formato válido.");

        RuleFor(request => request.Precio)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El precio debe ser mayor o igual a 0.");

        RuleFor(request => request.Stock)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El stock debe ser mayor o igual a 0.");
    }
}