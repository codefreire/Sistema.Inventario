import { screen, waitFor } from '@testing-library/react';
import { describe, expect, it, vi, beforeEach } from 'vitest';
import ProductosPage from '../../../../features/productos/pages/ProductosPage';
import * as productoService from '../../../../features/productos/services/productoService';
import { renderConProveedores } from '../../../setup/testUtils';

vi.mock('../../../../features/productos/services/productoService');

const mockNavigate = vi.fn();
vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom');
  return {
    ...actual,
    useNavigate: () => mockNavigate,
  };
});

describe('ProductosPage', () => {
  const productosEjemplo = [
    {
      id: '1',
      nombre: 'Laptop',
      descripcion: 'Laptop test',
      categoria: 'Tech',
      imagenUrl: 'https://test.com/img.jpg',
      precio: 1000,
      stock: 5,
    },
  ];

  beforeEach(() => {
    vi.clearAllMocks();
    vi.mocked(productoService.productoService.obtenerTodos).mockResolvedValueOnce(productosEjemplo);
  });

  it('DebeCargarProductos', async () => {
    renderConProveedores(<ProductosPage />);

    await waitFor(() => {
      expect(productoService.productoService.obtenerTodos).toHaveBeenCalled();
      expect(screen.getByText('Laptop')).toBeInTheDocument();
    });
  });

  it('DebeRenderizarTabla', async () => {
    renderConProveedores(<ProductosPage />);

    await waitFor(() => {
      expect(screen.getByText('Laptop')).toBeInTheDocument();
      expect(screen.getByText('Tech')).toBeInTheDocument();
    });
  });

  it('DebeRenderizarSinProductos', async () => {
    vi.mocked(productoService.productoService.obtenerTodos).mockResolvedValueOnce([]);

    renderConProveedores(<ProductosPage />);

    await waitFor(() => {
      expect(productoService.productoService.obtenerTodos).toHaveBeenCalled();
    });
  });
});
