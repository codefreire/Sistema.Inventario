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
    // ARRANGE: Activar timers simulados y renderizar una notificacion visible
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

    // ACT: Avanzar el tiempo simulado hasta el auto-cierre configurado
    vi.advanceTimersByTime(4000);

    // ASSERT: Verificar que el callback de cierre se invoque una sola vez
    expect(alCerrar).toHaveBeenCalledTimes(1);

    vi.useRealTimers();
  });

  /**
   * Verifica que cuando no esta visible, no se renderiza contenido en pantalla.
   */
  it('Renderizar_CuandoVisibleEsFalse_DebeRetornarNulo', () => {
    // ARRANGE: Renderizar la notificacion en estado oculto
    const alCerrar = vi.fn();

    renderConProveedores(
      <Notificacion
        mensaje="Sin mostrar"
        tipo="info"
        visible={false}
        onCerrar={alCerrar}
      />
    );

    // ACT: Consultar si existe el boton de cierre en el DOM
    const botonCerrar = screen.queryByRole('button');

    // ASSERT: Verificar que no se renderice contenido interactivo visible
    expect(botonCerrar).toBeNull();
  });
});
