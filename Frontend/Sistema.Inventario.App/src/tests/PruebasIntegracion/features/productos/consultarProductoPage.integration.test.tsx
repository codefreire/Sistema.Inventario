import { fireEvent, screen, waitFor } from '@testing-library/react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import ConsultarProductoPage from '../../../../features/productos/pages/ConsultarProductoPage';
import * as productoService from '../../../../features/productos/services/productoService';
import { renderConProveedores } from '../../../setup/testUtils';

vi.mock('../../../../features/productos/services/productoService');

const mockNavigate = vi.fn();
const mockParams = { id: 'prod-123' };

vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom');
  return {
    ...actual,
    useNavigate: () => mockNavigate,
    useParams: () => mockParams,
  };
});

describe('ConsultarProductoPage', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockNavigate.mockClear();

    vi.mocked(productoService.productoService.obtenerPorId).mockResolvedValue({
      id: 'prod-123',
      nombre: 'Mesa',
      descripcion: 'Mesa rectangular de madera',
      categoria: 'Hogar',
      imagenUrl: 'http://localhost:5261/uploads/mesa.png',
      precio: 70,
      stock: 5,
    });
  });

  it('DebeCargarYMostrarDatosDelProducto', async () => {
    // ARRANGE:
    renderConProveedores(<ConsultarProductoPage />);

    // ASSERT:
    await waitFor(() => {
      expect(productoService.productoService.obtenerPorId).toHaveBeenCalledWith('prod-123');
      expect(screen.getByDisplayValue('Mesa')).toBeInTheDocument();
      expect(screen.getByDisplayValue('Mesa rectangular de madera')).toBeInTheDocument();
      expect(screen.getByDisplayValue('Hogar')).toBeInTheDocument();
      expect(screen.getByDisplayValue('5')).toBeInTheDocument();
      expect(screen.getByRole('heading', { name: /consulta de producto/i })).toBeInTheDocument();
    });
  });

  it('VolverAProductos_CuandoSePresionaBoton_DebeNavegarAListado', async () => {
    // ARRANGE:
    renderConProveedores(<ConsultarProductoPage />);

    await waitFor(() => {
      expect(screen.getByRole('button', { name: /volver a productos/i })).toBeInTheDocument();
    });

    // ACT:
    fireEvent.click(screen.getByRole('button', { name: /volver a productos/i }));

    // ASSERT:
    expect(mockNavigate).toHaveBeenCalledWith('/productos');
  });

  it('IrAEdicion_CuandoSePresionaBoton_DebeNavegarAEditarProducto', async () => {
    // ARRANGE:
    renderConProveedores(<ConsultarProductoPage />);

    await waitFor(() => {
      expect(screen.getByRole('button', { name: /ir a edición/i })).toBeInTheDocument();
    });

    // ACT:
    fireEvent.click(screen.getByRole('button', { name: /ir a edición/i }));

    // ASSERT:
    expect(mockNavigate).toHaveBeenCalledWith('/productos/editar/prod-123');
  });

  it('ErrorAlConsultar_CuandoServicioFalla_DebeMostrarNotificacion', async () => {
    // ARRANGE:
    vi.mocked(productoService.productoService.obtenerPorId).mockRejectedValueOnce(new Error('Producto no encontrado'));

    renderConProveedores(<ConsultarProductoPage />);

    // ASSERT:
    await waitFor(() => {
      expect(screen.getByText(/producto no encontrado/i)).toBeInTheDocument();
      expect(screen.getByRole('button', { name: /volver a productos/i })).toBeInTheDocument();
    });
  });
});
