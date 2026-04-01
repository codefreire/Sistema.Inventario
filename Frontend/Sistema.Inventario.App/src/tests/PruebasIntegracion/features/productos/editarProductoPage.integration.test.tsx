import { fireEvent, screen, waitFor } from '@testing-library/react';
import { describe, expect, it, vi, beforeEach } from 'vitest';
import EditarProductoPage from '../../../../features/productos/pages/EditarProductoPage';
import * as productoService from '../../../../features/productos/services/productoService';
import { renderConProveedores } from '../../../setup/testUtils';

vi.mock('../../../../features/productos/services/productoService');

const mockNavigate = vi.fn();
const mockParams = { id: 'test-id' };

vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom');
  return {
    ...actual,
    useNavigate: () => mockNavigate,
    useParams: () => mockParams,
  };
});

describe('EditarProductoPage', () => {
  beforeEach(() => {
    vi.clearAllMocks();

    vi.mocked(productoService.productoService.obtenerPorId).mockResolvedValueOnce({
      id: 'test-id',
      nombre: 'Producto Test',
      descripcion: 'Desc',
      categoria: 'Cat',
      imagenUrl: 'https://test.com/test.jpg',
      precio: 100,
      stock: 1,
    });
  });

  it('DebeCargarProductoAlAbrir', async () => {
    renderConProveedores(<EditarProductoPage />);

    await waitFor(() => {
      expect(productoService.productoService.obtenerPorId).toHaveBeenCalledWith('test-id');
      expect(screen.getByLabelText(/nombre/i)).toBeInTheDocument();
    });
  });

  it('AlCancelarDebeNavegar', async () => {
    renderConProveedores(<EditarProductoPage />);

    await waitFor(() => {
      expect(screen.getByLabelText(/nombre/i)).toBeInTheDocument();
    });

    const botonCancelar = screen.getByRole('button', { name: /cancelar/i });
    fireEvent.click(botonCancelar);
    expect(mockNavigate).toHaveBeenCalledWith('/productos');
  });

  it('AlSubmitDebeCallService', async () => {
    vi.mocked(productoService.productoService.actualizar).mockResolvedValueOnce({
      id: 'test-id',
      nombre: 'Actualizado',
      descripcion: 'Desc',
      categoria: 'Cat',
      imagenUrl: 'https://test.com/test.jpg',
      precio: 100,
      stock: 1,
    });

    renderConProveedores(<EditarProductoPage />);

    await waitFor(() => {
      expect(screen.getByLabelText(/nombre/i)).toBeInTheDocument();
    });

    fireEvent.change(screen.getByLabelText(/nombre/i), { target: { value: 'Actualizado' } });

    fireEvent.click(screen.getByRole('button', { name: /actualizar/i }));

    await waitFor(() => {
      expect(productoService.productoService.actualizar).toHaveBeenCalledWith('test-id', expect.any(Object));
    });
  });
});
