import { describe, expect, it } from 'vitest';
import { formatearMoneda } from '../../../shared/utils/formatCurrency';

/**
 * Pruebas unitarias para utilidades compartidas de formato monetario.
 */
describe('formatearMoneda', () => {
    /**
     * Verifica que el formato aplicado sea el esperado para es-EC y USD.
     */
    it('FormatearMoneda_CuandoRecibeValorNumerico_DebeAplicarFormatoMonedaUsd', () => {
        // ARRANGE:
        const valor = 1234.5;
        const formatoEsperado = new Intl.NumberFormat('es-EC', {
            style: 'currency',
            currency: 'USD',
            minimumFractionDigits: 2,
        }).format(valor);

        // ACT:
        const resultado = formatearMoneda(valor);

        // ASSERT:
        expect(resultado).toBe(formatoEsperado);
    });
});
