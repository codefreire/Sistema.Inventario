using Sistema.Inventario.Producto.Aplicacion.DTOs.Requests;
using Sistema.Inventario.Producto.Aplicacion.Validators;

namespace Sistema.Inventario.Producto.Tests.PruebasUnitarias;

/// <summary>
/// Pruebas unitarias para validar ActualizarProductoValidator con diferentes escenarios de entrada
/// </summary>
public class ActualizarProductoValidatorTests
{
    /// <summary>
    /// Validador para Actualizar un Producto
    /// </summary>
    private readonly ActualizarProductoValidator _validator = new();

    /// <summary>
    /// Valida que el validador rechace una URL de imagen inválida
    /// </summary>
    [Fact]
    public void Validar_CuandoLaImagenNoEsValida_RetornaError()
    {
        // ARRANGE: Preparar un request con URL de imagen inválida
        ActualizarProductoRequest request = new()
        {
            Nombre = "Monitor 27",
            Descripcion = "Monitor profesional",
            Categoria = "Tecnologia",
            ImagenUrl = "imagen-local",
            Precio = 400,
            Stock = 3
        };

        // ACT: Ejecutar la validación
        FluentValidation.Results.ValidationResult resultado = _validator.Validate(request);

        // ASSERT: Verificar que hay error de URL inválida
        Assert.False(resultado.IsValid);
        Assert.Contains(resultado.Errors, error => error.ErrorMessage == "La Url de la imagen del producto no tiene un formato válido.");
    }

    /// <summary>
    /// Valida que el validador rechace valores negativos de precio y stock
    /// </summary>
    [Fact]
    public void Validar_CuandoPrecioYStockSonNegativos_RetornaErrores()
    {
        // ARRANGE: Preparar un request con precio y stock negativos
        ActualizarProductoRequest request = new()
        {
            Nombre = "Monitor 27",
            Descripcion = "Monitor profesional",
            Categoria = "Tecnologia",
            ImagenUrl = "https://sistemainventario.com/productos/monitor.png",
            Precio = -10,
            Stock = -1
        };

        // ACT: Ejecutar la validación
        FluentValidation.Results.ValidationResult resultado = _validator.Validate(request);

        // ASSERT: Verificar que hay errores de valores negativos
        Assert.False(resultado.IsValid);
        Assert.Contains(resultado.Errors, error => error.ErrorMessage == "El precio debe ser mayor o igual a 0.");
        Assert.Contains(resultado.Errors, error => error.ErrorMessage == "El stock debe ser mayor o igual a 0.");
    }
}