import { describe, expect, it } from 'vitest';
import { tieneErrores, validarTransaccion } from '../../../../features/transacciones/validators/transaccionValidator';
import type { CrearTransaccionRequest } from '../../../../features/transacciones/types/Transaccion';

/**
 * Pruebas unitarias para la validacion de transacciones.
 */
describe('validarTransaccion', () => {
    /**
     * Verifica que una transaccion valida de compra no retorne errores.
     */
    it('ValidarTransaccion_CuandoLaTransaccionEsValida_DebeRetornarSinErrores', () => {
        // ARRANGE: Preparar una transaccion de compra con datos validos
        const transaccionValida: CrearTransaccionRequest = {
            tipoTransaccion: 'Compra',
            productoId: 'prod-001',
            cantidad: 5,
            precioUnitario: 12.5,
            detalle: 'Compra mensual',
        };

        // ACT: Ejecutar la validacion de la transaccion
        const resultado = validarTransaccion(transaccionValida);

        // ASSERT: Verificar que no se produzcan errores de validacion
        expect(resultado).toEqual({});
        expect(tieneErrores(resultado)).toBe(false);
    });

    /**
     * Verifica que una venta superior al stock disponible falle la validacion.
     */
    it('ValidarTransaccion_CuandoLaVentaSuperaElStock_DebeRetornarErrorDeCantidad', () => {
        // ARRANGE: Preparar una venta invalida con cantidad superior al stock disponible
        const transaccionInvalida: CrearTransaccionRequest = {
            tipoTransaccion: 'Venta',
            productoId: 'prod-001',
            cantidad: 8,
            precioUnitario: 12.5,
            detalle: 'Venta mostrador',
        };
        const stockDisponible = 3;

        // ACT: Ejecutar la validacion considerando el stock disponible
        const resultado = validarTransaccion(transaccionInvalida, stockDisponible);

        // ASSERT: Verificar que se retorne el error esperado en la cantidad
        expect(resultado.cantidad).toBe('No puede vender más del stock disponible (3 unidades).');
        expect(tieneErrores(resultado)).toBe(true);
    });
});
