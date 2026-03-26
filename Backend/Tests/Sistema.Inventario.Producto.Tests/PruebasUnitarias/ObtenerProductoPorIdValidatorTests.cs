using Sistema.Inventario.Producto.Aplicacion.DTOs.Requests;
using Sistema.Inventario.Producto.Aplicacion.Validators;

namespace Sistema.Inventario.Producto.Tests.PruebasUnitarias;

/// <summary>
/// Pruebas unitarias para validar ObtenerProductoPorIdValidator con diferentes escenarios de entrada
/// </summary>
public class ObtenerProductoPorIdValidatorTests
{
    /// <summary>
    /// Validador para Obtener un Producto por Id
    /// </summary>
    private readonly ObtenerProductoPorIdValidator _validator = new();

    /// <summary>
    /// Valida que el validador rechace un ID vacío
    /// </summary>
    [Fact]
    public void Validar_CuandoElIdEsVacio_RetornaError()
    {
        // ARRANGE: Preparar un request con ID vacío
        ObtenerProductoPorIdRequest request = new()
        {
            Id = Guid.Empty
        };

        // ACT: Ejecutar la validación
        FluentValidation.Results.ValidationResult resultado = _validator.Validate(request);

        // ASSERT: Verificar que hay error de ID requerido
        Assert.False(resultado.IsValid);
        Assert.Contains(resultado.Errors, error => error.ErrorMessage == "El Id del producto es obligatorio.");
    }

    /// <summary>
    /// Valida que el validador acepte un ID válido sin errores
    /// </summary>
    [Fact]
    public void Validar_CuandoElIdEsValido_NoDebeRetornarErrores()
    {
        // ARRANGE: Preparar un request con ID válido (no vacío)
        ObtenerProductoPorIdRequest request = new()
        {
            Id = Guid.NewGuid()
        };

        // ACT: Ejecutar la validación
        FluentValidation.Results.ValidationResult resultado = _validator.Validate(request);

        // ASSERT: Verificar que no hay errores
        Assert.True(resultado.IsValid);
        Assert.Empty(resultado.Errors);
    }
}