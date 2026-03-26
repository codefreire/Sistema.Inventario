import { fireEvent, screen } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import FiltrosTransacciones from '../../../../features/transacciones/components/FiltrosTransacciones';
import type { FiltrosTransaccion } from '../../../../features/transacciones/types/Transaccion';
import type { Producto } from '../../../../features/productos/types/Producto';
import { renderConProveedores } from '../../../setup/testUtils';

/**
 * Pruebas de integracion para FiltrosTransacciones.
 */
describe('FiltrosTransacciones', () => {
  /**
   * Verifica que al seleccionar un tipo se notifique el filtro actualizado.
   */
  it('CambiarTipo_CuandoSeSeleccionaVenta_DebeEmitirTipoTransaccion', () => {
    // ARRANGE:
    const alCambiarFiltros = vi.fn();
    const filtrosIniciales: FiltrosTransaccion = {};
    const productos: Producto[] = [];

    renderConProveedores(
      <FiltrosTransacciones
        filtros={filtrosIniciales}
        onCambiarFiltros={alCambiarFiltros}
        productos={productos}
      />
    );

    const selectTipo = screen.getAllByRole('combobox')[0];

    // ACT:
    fireEvent.change(selectTipo, { target: { value: 'Venta' } });

    // ASSERT:
    expect(alCambiarFiltros).toHaveBeenCalledTimes(1);
    expect(alCambiarFiltros).toHaveBeenCalledWith({ tipoTransaccion: 'Venta' });
  });

  /**
   * Verifica que al limpiar filtros se emita un objeto vacio.
   */
  it('LimpiarFiltros_CuandoSePresionaBoton_DebeEmitirObjetoVacio', () => {
    // ARRANGE:
    const alCambiarFiltros = vi.fn();
    const filtrosIniciales: FiltrosTransaccion = { tipoTransaccion: 'Compra' };
    const productos: Producto[] = [];

    renderConProveedores(
      <FiltrosTransacciones
        filtros={filtrosIniciales}
        onCambiarFiltros={alCambiarFiltros}
        productos={productos}
      />
    );

    const botonLimpiar = screen.getByRole('button', { name: 'Limpiar Filtros' });

    // ACT:
    fireEvent.click(botonLimpiar);

    // ASSERT:
    expect(alCambiarFiltros).toHaveBeenCalledTimes(1);
    expect(alCambiarFiltros).toHaveBeenCalledWith({});
  });
});
