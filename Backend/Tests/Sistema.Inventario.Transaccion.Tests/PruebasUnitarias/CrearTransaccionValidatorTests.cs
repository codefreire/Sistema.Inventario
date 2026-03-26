using Sistema.Inventario.Transaccion.Aplicacion.DTOs.Requests;
using Sistema.Inventario.Transaccion.Aplicacion.Validators;

namespace Sistema.Inventario.Transaccion.Tests.PruebasUnitarias;

/// <summary>
/// Pruebas unitarias para validar CrearTransaccionValidator en distintos escenarios de entrada
/// </summary>
public class CrearTransaccionValidatorTests
{
    /// <summary>
    /// Validador para Crear una Transacción
    /// </summary>
    private readonly CrearTransaccionValidator _validador = new();

    /// <summary>
    /// Valida que el validador acepte una solicitud válida sin retornar errores
    /// </summary>
    [Fact]
    public void Validar_CuandoLaSolicitudEsValida_NoDebeRetornarErrores()
    {
        // ARRANGE: Preparar una solicitud válida para crear una transacción
        CrearTransaccionRequest request = new()
        {
            TipoTransaccion = "Compra",
            ProductoId = Guid.NewGuid(),
            Cantidad = 3,
            PrecioUnitario = 25.50m,
            Detalle = "Compra inicial de inventario"
        };

        // ACT: Ejecutar la validación de la solicitud
        FluentValidation.Results.ValidationResult resultado = _validador.Validate(request);

        // ASSERT: Verificar que la validación sea satisfactoria
        Assert.True(resultado.IsValid);
        Assert.Empty(resultado.Errors);
    }

    /// <summary>
    /// Valida que el validador rechace una solicitud inválida con los mensajes esperados
    /// </summary>
    [Fact]
    public void Validar_CuandoLaSolicitudEsInvalida_RetornaErroresEsperados()
    {
        // ARRANGE: Preparar una solicitud inválida con múltiples reglas incumplidas
        CrearTransaccionRequest request = new()
        {
            TipoTransaccion = "Transferencia",
            ProductoId = Guid.Empty,
            Cantidad = 0,
            PrecioUnitario = -1,
            Detalle = string.Empty
        };

        // ACT: Ejecutar la validación de la solicitud
        FluentValidation.Results.ValidationResult resultado = _validador.Validate(request);

        // ASSERT: Verificar que se reporten los errores esperados
        Assert.False(resultado.IsValid);
        Assert.Contains(resultado.Errors, error => error.ErrorMessage == "El tipo de transacción debe ser Compra o Venta.");
        Assert.Contains(resultado.Errors, error => error.ErrorMessage == "El Id del producto es obligatorio.");
        Assert.Contains(resultado.Errors, error => error.ErrorMessage == "La cantidad debe ser mayor a 0.");
        Assert.Contains(resultado.Errors, error => error.ErrorMessage == "El precio unitario debe ser mayor o igual a 0.");
        Assert.Contains(resultado.Errors, error => error.ErrorMessage == "El detalle de la transacción es obligatorio.");
    }
}
