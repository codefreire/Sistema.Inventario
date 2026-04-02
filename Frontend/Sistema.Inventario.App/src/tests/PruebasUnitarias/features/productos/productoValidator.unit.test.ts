import { describe, expect, it } from 'vitest';
import { tieneErrores, validarProducto } from '../../../../features/productos/validators/productoValidator';
import type { CrearProductoRequest } from '../../../../features/productos/types/Producto';

/**
 * Pruebas unitarias para la validacion de productos.
 */
describe('validarProducto', () => {
    /**
     * Verifica que un producto con datos validos no genere errores.
     */
    it('ValidarProducto_CuandoElProductoEsValido_DebeRetornarSinErrores', () => {
        // ARRANGE: Preparar un producto con todos los campos requeridos y valores validos
        const productoValido: CrearProductoRequest = {
            nombre: 'Teclado Mecanico',
            descripcion: 'Teclado para oficina',
            categoria: 'Perifericos',
            imagenUrl: 'https://cdn.ejemplo.com/teclado.png',
            precio: 99.99,
            stock: 10,
        };

        // ACT: Ejecutar la validacion del producto
        const resultado = validarProducto(productoValido);

        // ASSERT: Verificar que no existan errores de validacion
        expect(resultado).toEqual({});
        expect(tieneErrores(resultado)).toBe(false);
    });

    /**
     * Verifica que se retornen errores cuando faltan campos obligatorios.
     */
    it('ValidarProducto_CuandoFaltanCamposObligatorios_DebeRetornarErroresPorCampo', () => {
        // ARRANGE: Preparar un producto invalido con todos los campos obligatorios vacios
        const productoInvalido = {
            nombre: '',
            descripcion: '',
            categoria: '',
            imagenUrl: '',
            precio: null,
            stock: null,
        } as unknown as CrearProductoRequest;

        // ACT: Ejecutar la validacion del producto invalido
        const resultado = validarProducto(productoInvalido);

        // ASSERT: Verificar que se retornen los mensajes de error esperados por campo
        expect(resultado.nombre).toBe('El nombre es obligatorio.');
        expect(resultado.descripcion).toBe('La descripción es obligatoria.');
        expect(resultado.categoria).toBe('La categoría es obligatoria.');
        expect(resultado.imagenUrl).toBe('La URL de la imagen es obligatoria.');
        expect(resultado.precio).toBe('El precio es obligatorio.');
        expect(resultado.stock).toBe('El stock es obligatorio.');
        expect(tieneErrores(resultado)).toBe(true);
    });
});
