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
        // ARRANGE:
        const transaccionValida: CrearTransaccionRequest = {
            tipoTransaccion: 'Compra',
            productoId: 'prod-001',
            cantidad: 5,
            precioUnitario: 12.5,
            detalle: 'Compra mensual',
        };

        // ACT:
        const resultado = validarTransaccion(transaccionValida);

        // ASSERT:
        expect(resultado).toEqual({});
        expect(tieneErrores(resultado)).toBe(false);
    });

    /**
     * Verifica que una venta superior al stock disponible falle la validacion.
     */
    it('ValidarTransaccion_CuandoLaVentaSuperaElStock_DebeRetornarErrorDeCantidad', () => {
        // ARRANGE:
        const transaccionInvalida: CrearTransaccionRequest = {
            tipoTransaccion: 'Venta',
            productoId: 'prod-001',
            cantidad: 8,
            precioUnitario: 12.5,
            detalle: 'Venta mostrador',
        };
        const stockDisponible = 3;

        // ACT:
        const resultado = validarTransaccion(transaccionInvalida, stockDisponible);

        // ASSERT:
        expect(resultado.cantidad).toBe('No puede vender más del stock disponible (3 unidades).');
        expect(tieneErrores(resultado)).toBe(true);
    });
});
