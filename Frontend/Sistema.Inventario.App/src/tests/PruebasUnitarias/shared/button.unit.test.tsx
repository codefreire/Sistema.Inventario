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
        // ARRANGE:
        renderConProveedores(<Button variante="peligro">Eliminar</Button>);

        // ACT:
        const boton = screen.getByRole('button', { name: 'Eliminar' });

        // ASSERT:
        expect(boton).toHaveClass('btn');
        expect(boton).toHaveClass('btn-peligro');
    });

    /**
     * Verifica que al hacer click se invoque el callback onClick.
     */
    it('Button_CuandoSeHaceClick_DebeInvocarOnClickUnaVez', () => {
        // ARRANGE:
        const alHacerClick = vi.fn();
        renderConProveedores(
            <Button variante="primario" onClick={alHacerClick}>
                Guardar
            </Button>
        );
        const boton = screen.getByRole('button', { name: 'Guardar' });

        // ACT:
        fireEvent.click(boton);

        // ASSERT:
        expect(alHacerClick).toHaveBeenCalledTimes(1);
    });
});
