import { fireEvent, screen } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import Button from '../../../shared/components/Button';
import { renderConProveedores } from '../../setup/testUtils';

/**
 * Pruebas unitarias del componente Button.
 */
describe('Button', () => {
    /**
     * Verifica que el boton renderice la clase segun la variante indicada.
     */
    it('Button_CuandoSeRenderizaConVariantePeligro_DebeAplicarClaseCorrespondiente', () => {
        // ARRANGE: Renderizar el boton con la variante de peligro
        renderConProveedores(<Button variante="peligro">Eliminar</Button>);

        // ACT: Obtener el boton renderizado desde el DOM
        const boton = screen.getByRole('button', { name: 'Eliminar' });

        // ASSERT: Verificar que se apliquen las clases visuales esperadas
        expect(boton).toHaveClass('btn');
        expect(boton).toHaveClass('btn-peligro');
    });

    /**
     * Verifica que al hacer click se invoque el callback onClick.
     */
    it('Button_CuandoSeHaceClick_DebeInvocarOnClickUnaVez', () => {
        // ARRANGE: Renderizar el boton con un callback espia para el evento click
        const alHacerClick = vi.fn();
        renderConProveedores(
            <Button variante="primario" onClick={alHacerClick}>
                Guardar
            </Button>
        );
        const boton = screen.getByRole('button', { name: 'Guardar' });

        // ACT: Ejecutar un clic sobre el boton renderizado
        fireEvent.click(boton);

        // ASSERT: Verificar que el callback se invoque exactamente una vez
        expect(alHacerClick).toHaveBeenCalledTimes(1);
    });
});
