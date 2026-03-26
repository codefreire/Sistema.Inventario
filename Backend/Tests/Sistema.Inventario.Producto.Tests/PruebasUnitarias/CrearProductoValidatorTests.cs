using Sistema.Inventario.Producto.Aplicacion.DTOs.Requests;
using Sistema.Inventario.Producto.Aplicacion.Validators;

namespace Sistema.Inventario.Producto.Tests.PruebasUnitarias;

/// <summary>
/// Pruebas unitarias para validar CrearProductoValidator con diferentes escenarios de entrada
/// </summary>
public class CrearProductoValidatorTests
{
    /// <summary>
    /// Validador para Crear un Producto
    /// </summary>
    private readonly CrearProductoValidator _validator = new();

    /// <summary>
    /// Valida que el validador acepte un request válido sin errores
    /// </summary>
    [Fact]
    public void Validar_CuandoElRequestEsValido_NoDebeRetornarErrores()
    {
        // ARRANGE: Preparar un request válido con todos los datos requeridos
        CrearProductoRequest request = new()
        {
            Nombre = "Laptop Dell",
            Descripcion = "Laptop para oficina",
            Categoria = "Tecnologia",
            ImagenUrl = "https://sistemainventario.com/productos/laptop-dell.png",
            Precio = 1250.50m,
            Stock = 10
        };

        // ACT: Ejecutar la validación
        FluentValidation.Results.ValidationResult resultado = _validator.Validate(request);

        // ASSERT: Verificar que no hay errores
        Assert.True(resultado.IsValid);
        Assert.Empty(resultado.Errors);
    }

    /// <summary>
    /// Valida que el validador rechace un request inválido con los errores esperados
    /// </summary>
    [Fact]
    public void Validar_CuandoElRequestEsInvalido_RetornaErroresEsperados()
    {
        // ARRANGE: Preparar un request inválido con múltiples errores
        CrearProductoRequest request = new()
        {
            Nombre = string.Empty,
            Descripcion = string.Empty,
            Categoria = string.Empty,
            ImagenUrl = "url-invalida",
            Precio = -1,
            Stock = -5
        };

        // ACT: Ejecutar la validación
        FluentValidation.Results.ValidationResult resultado = _validator.Validate(request);

        // ASSERT: Verificar que todos los errores esperados están presentes
        Assert.False(resultado.IsValid);
        Assert.Contains(resultado.Errors, error => error.ErrorMessage == "El nombre del producto es obligatorio.");
        Assert.Contains(resultado.Errors, error => error.ErrorMessage == "La descripción del producto es obligatoria.");
        Assert.Contains(resultado.Errors, error => error.ErrorMessage == "La categoría del producto es obligatoria.");
        Assert.Contains(resultado.Errors, error => error.ErrorMessage == "La Url de la imagen del producto no tiene un formato válido.");
        Assert.Contains(resultado.Errors, error => error.ErrorMessage == "El precio debe ser mayor o igual a 0.");
        Assert.Contains(resultado.Errors, error => error.ErrorMessage == "El stock debe ser mayor o igual a 0.");
    }
}