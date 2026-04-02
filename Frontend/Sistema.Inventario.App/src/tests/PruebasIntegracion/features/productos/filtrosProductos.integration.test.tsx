import { fireEvent, screen } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import FiltrosProductos from '../../../../features/productos/components/FiltrosProductos';
import type { FiltrosProducto } from '../../../../features/productos/types/Producto';
import { renderConProveedores } from '../../../setup/testUtils';

/**
 * Pruebas de integracion para FiltrosProductos.
 */
describe('FiltrosProductos', () => {
    /**
     * Verifica que al cambiar el nombre se notifique el filtro actualizado.
     */
    it('CambiarNombre_CuandoSeIngresaTexto_DebeEmitirFiltrosActualizados', () => {
        // ARRANGE: Renderizar el componente con filtros vacios y callback espia
        const alCambiarFiltros = vi.fn();
        const filtrosIniciales: FiltrosProducto = {};

        renderConProveedores(
            <FiltrosProductos filtros={filtrosIniciales} onCambiarFiltros={alCambiarFiltros} />
        );

        const inputNombre = screen.getByPlaceholderText('Buscar por nombre...');

        // ACT: Ingresar un valor en el campo de nombre
        fireEvent.change(inputNombre, { target: { value: 'Laptop' } });

        // ASSERT: Verificar que se emita el filtro actualizado con el nombre ingresado
        expect(alCambiarFiltros).toHaveBeenCalledTimes(1);
        expect(alCambiarFiltros).toHaveBeenCalledWith({ nombre: 'Laptop' });
    });

    /**
     * Verifica que al limpiar filtros se emita un objeto vacio.
     */
    it('LimpiarFiltros_CuandoSePresionaBoton_DebeEmitirObjetoVacio', () => {
        // ARRANGE: Renderizar el componente con filtros cargados previamente
        const alCambiarFiltros = vi.fn();
        const filtrosIniciales: FiltrosProducto = { nombre: 'Mouse', stockMin: 1 };

        renderConProveedores(
            <FiltrosProductos filtros={filtrosIniciales} onCambiarFiltros={alCambiarFiltros} />
        );

        const botonLimpiar = screen.getByRole('button', { name: 'Limpiar Filtros' });

        // ACT: Hacer clic en el boton para limpiar filtros
        fireEvent.click(botonLimpiar);

        // ASSERT: Verificar que se emita un objeto vacio como nuevo estado de filtros
        expect(alCambiarFiltros).toHaveBeenCalledTimes(1);
        expect(alCambiarFiltros).toHaveBeenCalledWith({});
    });
});
