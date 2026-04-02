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
        // ARRANGE: Preparar un valor numerico y el formato esperado con Intl.NumberFormat
        const valor = 1234.5;
        const formatoEsperado = new Intl.NumberFormat('es-EC', {
            style: 'currency',
            currency: 'USD',
            minimumFractionDigits: 2,
        }).format(valor);

        // ACT: Ejecutar la utilidad de formato monetario
        const resultado = formatearMoneda(valor);

        // ASSERT: Verificar que el valor se formatee exactamente como USD en es-EC
        expect(resultado).toBe(formatoEsperado);
    });
});
