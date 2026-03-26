using Sistema.Inventario.Transaccion.Aplicacion.DTOs.Requests;
using Sistema.Inventario.Transaccion.Aplicacion.Validators;

namespace Sistema.Inventario.Transaccion.Tests.PruebasUnitarias;

/// <summary>
/// Pruebas unitarias para validar ActualizarTransaccionValidator con diferentes combinaciones de datos
/// </summary>
public class ActualizarTransaccionValidatorTests
{
    /// <summary>
    /// Validador para Actualizar una Transacción
    /// </summary>
    private readonly ActualizarTransaccionValidator _validador = new();

    /// <summary>
    /// Valida que el validador rechace un tipo de transacción fuera del dominio permitido
    /// </summary>
    [Fact]
    public void Validar_CuandoElTipoTransaccionNoEsPermitido_RetornaError()
    {
        // ARRANGE: Preparar una solicitud con tipo de transacción inválido
        ActualizarTransaccionRequest request = new()
        {
            TipoTransaccion = "Ajuste",
            ProductoId = Guid.NewGuid(),
            Cantidad = 5,
            PrecioUnitario = 12.75m,
            Detalle = "Actualización de prueba"
        };

        // ACT: Ejecutar la validación de la solicitud
        FluentValidation.Results.ValidationResult resultado = _validador.Validate(request);

        // ASSERT: Verificar que se informe error por tipo no permitido
        Assert.False(resultado.IsValid);
        Assert.Contains(resultado.Errors, error => error.ErrorMessage == "El tipo de transacción debe ser Compra o Venta.");
    }

    /// <summary>
    /// Valida que el validador rechace valores inválidos para producto, cantidad y precio
    /// </summary>
    [Fact]
    public void Validar_CuandoLosValoresNumericosSonInvalidos_RetornaErrores()
    {
        // ARRANGE: Preparar una solicitud con identificador vacío y valores numéricos inválidos
        ActualizarTransaccionRequest request = new()
        {
            TipoTransaccion = "Venta",
            ProductoId = Guid.Empty,
            Cantidad = -2,
            PrecioUnitario = -10,
            Detalle = "Venta inválida"
        };

        // ACT: Ejecutar la validación de la solicitud
        FluentValidation.Results.ValidationResult resultado = _validador.Validate(request);

        // ASSERT: Verificar que se reporten los errores esperados
        Assert.False(resultado.IsValid);
        Assert.Contains(resultado.Errors, error => error.ErrorMessage == "El Id del producto es obligatorio.");
        Assert.Contains(resultado.Errors, error => error.ErrorMessage == "La cantidad debe ser mayor a 0.");
        Assert.Contains(resultado.Errors, error => error.ErrorMessage == "El precio unitario debe ser mayor o igual a 0.");
    }
}
