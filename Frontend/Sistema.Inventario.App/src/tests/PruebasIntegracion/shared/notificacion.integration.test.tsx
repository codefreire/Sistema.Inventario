import { screen } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import Notificacion from '../../../shared/components/Notificacion';
import { renderConProveedores } from '../../setup/testUtils';

/**
 * Pruebas de integracion para el componente Notificacion.
 */
describe('Notificacion', () => {
  /**
   * Verifica que la notificacion visible se cierre automaticamente tras 4 segundos.
   */
  it('AutoCerrar_CuandoLaNotificacionEsVisible_DebeInvocarOnCerrar', async () => {
    // ARRANGE:
    vi.useFakeTimers();
    const alCerrar = vi.fn();

    renderConProveedores(
      <Notificacion
        mensaje="Guardado correctamente"
        tipo="exito"
        visible={true}
        onCerrar={alCerrar}
      />
    );

    // ACT:
    vi.advanceTimersByTime(4000);

    // ASSERT:
    expect(alCerrar).toHaveBeenCalledTimes(1);

    vi.useRealTimers();
  });

  /**
   * Verifica que cuando no esta visible, no se renderiza contenido en pantalla.
   */
  it('Renderizar_CuandoVisibleEsFalse_DebeRetornarNulo', () => {
    // ARRANGE:
    const alCerrar = vi.fn();

    renderConProveedores(
      <Notificacion
        mensaje="Sin mostrar"
        tipo="info"
        visible={false}
        onCerrar={alCerrar}
      />
    );

    // ACT:
    const botonCerrar = screen.queryByRole('button');

    // ASSERT:
    expect(botonCerrar).toBeNull();
  });
});
