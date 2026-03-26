using Sistema.Inventario.Transaccion.Aplicacion.DTOs.Requests;
using Sistema.Inventario.Transaccion.Aplicacion.Validators;

namespace Sistema.Inventario.Transaccion.Tests.PruebasUnitarias;

/// <summary>
/// Pruebas unitarias para validar ObtenerTransaccionPorIdValidator con valores de identificador válidos e inválidos
/// </summary>
public class ObtenerTransaccionPorIdValidatorTests
{
    /// <summary>
    /// Validador para Obtener una Transacción por Id
    /// </summary>
    private readonly ObtenerTransaccionPorIdValidator _validador = new();

    /// <summary>
    /// Valida que el validador rechace un identificador vacío
    /// </summary>
    [Fact]
    public void Validar_CuandoElIdEsVacio_RetornaError()
    {
        // ARRANGE: Preparar una solicitud con identificador vacío
        ObtenerTransaccionPorIdRequest request = new()
        {
            Id = Guid.Empty
        };

        // ACT: Ejecutar la validación de la solicitud
        FluentValidation.Results.ValidationResult resultado = _validador.Validate(request);

        // ASSERT: Verificar que exista error de identificador obligatorio
        Assert.False(resultado.IsValid);
        Assert.Contains(resultado.Errors, error => error.ErrorMessage == "El Id de la transacción es obligatorio.");
    }

    /// <summary>
    /// Valida que el validador acepte un identificador válido sin errores
    /// </summary>
    [Fact]
    public void Validar_CuandoElIdEsValido_NoDebeRetornarErrores()
    {
        // ARRANGE: Preparar una solicitud con identificador válido
        ObtenerTransaccionPorIdRequest request = new()
        {
            Id = Guid.NewGuid()
        };

        // ACT: Ejecutar la validación de la solicitud
        FluentValidation.Results.ValidationResult resultado = _validador.Validate(request);

        // ASSERT: Verificar que la validación sea satisfactoria
        Assert.True(resultado.IsValid);
        Assert.Empty(resultado.Errors);
    }
}
