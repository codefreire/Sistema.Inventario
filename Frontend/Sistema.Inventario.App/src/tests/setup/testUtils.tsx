import type { ReactElement } from 'react';
import { render } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';

/**
 * Renderiza componentes de prueba con proveedores base.
 * Incluye enrutador para componentes que dependen de navegacion.
 */
export function renderConProveedores(ui: ReactElement) {
    return render(<MemoryRouter>{ui}</MemoryRouter>);
}
