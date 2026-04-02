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
    // ARRANGE: Renderizar el componente con filtros vacios y lista de productos vacia
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

    // ACT: Seleccionar el tipo de transaccion Venta en el combo
    fireEvent.change(selectTipo, { target: { value: 'Venta' } });

    // ASSERT: Verificar que se emita el filtro con el tipo de transaccion seleccionado
    expect(alCambiarFiltros).toHaveBeenCalledTimes(1);
    expect(alCambiarFiltros).toHaveBeenCalledWith({ tipoTransaccion: 'Venta' });
  });

  /**
   * Verifica que al limpiar filtros se emita un objeto vacio.
   */
  it('LimpiarFiltros_CuandoSePresionaBoton_DebeEmitirObjetoVacio', () => {
    // ARRANGE: Renderizar el componente con un filtro inicial ya aplicado
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

    // ACT: Hacer clic en el boton para limpiar el formulario de filtros
    fireEvent.click(botonLimpiar);

    // ASSERT: Verificar que se notifique un objeto vacio como nuevo filtro
    expect(alCambiarFiltros).toHaveBeenCalledTimes(1);
    expect(alCambiarFiltros).toHaveBeenCalledWith({});
  });
});
