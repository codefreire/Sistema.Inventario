import { fireEvent, screen, waitFor } from '@testing-library/react';
import { describe, expect, it, vi, beforeEach } from 'vitest';
import CrearProductoPage from '../../../../features/productos/pages/CrearProductoPage';
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

describe('CrearProductoPage', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockNavigate.mockClear();
  });

  it('DebeRenderizarFormularioCompleto', () => {
    renderConProveedores(<CrearProductoPage />);
    expect(screen.getByRole('heading', { name: /crear producto/i })).toBeInTheDocument();
    expect(screen.getByLabelText(/nombre/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/descripción/i)).toBeInTheDocument();
  });

  it('AlCancelarDebeNavegar', () => {
    renderConProveedores(<CrearProductoPage />);
    const botonCancelar = screen.getByRole('button', { name: /cancelar/i });
    fireEvent.click(botonCancelar);
    expect(mockNavigate).toHaveBeenCalledWith('/productos');
  });

  it('AlSubmitConImagenDebeCallService', async () => {
    vi.mocked(productoService.productoService.crear).mockResolvedValueOnce({
      id: '1',
      nombre: 'Test',
      descripcion: 'Test',
      categoria: 'Test',
      imagenUrl: 'https://test.com/test.jpg',
      precio: 100,
      stock: 1,
    });

    renderConProveedores(<CrearProductoPage />);

    fireEvent.change(screen.getByLabelText(/nombre/i), { target: { value: 'Test' } });
    fireEvent.change(screen.getByLabelText(/descripción/i), { target: { value: 'Test' } });
    fireEvent.change(screen.getByLabelText(/categoría/i), { target: { value: 'Test' } });
    // Imagen requiere URL válida - usar getByPlaceholder ya que no tiene label
    const inputImagen = screen.getByPlaceholderText(/dominio/i);
    fireEvent.change(inputImagen, { target: { value: 'https://test.com/test.jpg' } });

    const inputs = screen.getAllByRole('spinbutton');
    fireEvent.change(inputs[0], { target: { value: '100' } });
    fireEvent.change(inputs[1], { target: { value: '1' } });

    fireEvent.click(screen.getByRole('button', { name: /crear producto/i }));

    await waitFor(
      () => {
        expect(productoService.productoService.crear).toHaveBeenCalled();
      },
      { timeout: 3000 }
    );
  });
});
