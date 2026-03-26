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
        // ARRANGE:
        const productoValido: CrearProductoRequest = {
            nombre: 'Teclado Mecanico',
            descripcion: 'Teclado para oficina',
            categoria: 'Perifericos',
            imagenUrl: 'https://cdn.ejemplo.com/teclado.png',
            precio: 99.99,
            stock: 10,
        };

        // ACT:
        const resultado = validarProducto(productoValido);

        // ASSERT:
        expect(resultado).toEqual({});
        expect(tieneErrores(resultado)).toBe(false);
    });

    /**
     * Verifica que se retornen errores cuando faltan campos obligatorios.
     */
    it('ValidarProducto_CuandoFaltanCamposObligatorios_DebeRetornarErroresPorCampo', () => {
        // ARRANGE:
        const productoInvalido = {
            nombre: '',
            descripcion: '',
            categoria: '',
            imagenUrl: '',
            precio: null,
            stock: null,
        } as unknown as CrearProductoRequest;

        // ACT:
        const resultado = validarProducto(productoInvalido);

        // ASSERT:
        expect(resultado.nombre).toBe('El nombre es obligatorio.');
        expect(resultado.descripcion).toBe('La descripción es obligatoria.');
        expect(resultado.categoria).toBe('La categoría es obligatoria.');
        expect(resultado.imagenUrl).toBe('La URL de la imagen es obligatoria.');
        expect(resultado.precio).toBe('El precio es obligatorio.');
        expect(resultado.stock).toBe('El stock es obligatorio.');
        expect(tieneErrores(resultado)).toBe(true);
    });
});
