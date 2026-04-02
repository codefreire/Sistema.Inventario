import { fireEvent, screen, waitFor } from '@testing-library/react';
import { describe, expect, it, vi, beforeEach } from 'vitest';
import EditarProductoPage from '../../../../features/productos/pages/EditarProductoPage';
import * as productoService from '../../../../features/productos/services/productoService';
import { renderConProveedores } from '../../../setup/testUtils';

/**
 * Mock del servicio de productos para controlar las respuestas de carga y actualizacion.
 */
vi.mock('../../../../features/productos/services/productoService');

const mockNavigate = vi.fn();
const mockParams = { id: 'test-id' };

/**
 * Mock de react-router-dom para simular navegacion y parametros de ruta.
 */
vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom');
  return {
    ...actual,
    useNavigate: () => mockNavigate,
    useParams: () => mockParams,
  };
});

/**
 * Pruebas de integracion para la pagina EditarProductoPage.
 */
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

  /**
   * Verifica que al abrir la pagina se carguen los datos del producto.
   */
  it('DebeCargarProductoAlAbrir', async () => {
    // ACT: Esperar la carga inicial del formulario de edicion
    renderConProveedores(<EditarProductoPage />);

    // ASSERT: Verificar que se consulte el producto y se muestre el formulario
    await waitFor(() => {
      expect(productoService.productoService.obtenerPorId).toHaveBeenCalledWith('test-id');
      expect(screen.getByLabelText(/nombre/i)).toBeInTheDocument();
    });
  });

  /**
   * Verifica que el boton cancelar redirija al listado de productos.
   */
  it('AlCancelarDebeNavegar', async () => {
    // ACT: Presionar el boton cancelar en el formulario de edicion
    renderConProveedores(<EditarProductoPage />);

    await waitFor(() => {
      expect(screen.getByLabelText(/nombre/i)).toBeInTheDocument();
    });

    const botonCancelar = screen.getByRole('button', { name: /cancelar/i });
    fireEvent.click(botonCancelar);

    // ASSERT: Verificar que se redirija al listado de productos
    expect(mockNavigate).toHaveBeenCalledWith('/productos');
  });

  /**
   * Verifica que al enviar el formulario se invoque la actualizacion del servicio.
   */
  it('AlSubmitDebeCallService', async () => {
    // ARRANGE: Configurar la respuesta del servicio y preparar el escenario de actualizacion
    vi.mocked(productoService.productoService.actualizar).mockResolvedValueOnce({
      id: 'test-id',
      nombre: 'Actualizado',
      descripcion: 'Desc',
      categoria: 'Cat',
      imagenUrl: 'https://test.com/test.jpg',
      precio: 100,
      stock: 1,
    });

    // ACT: Modificar el nombre del producto y enviar el formulario
    renderConProveedores(<EditarProductoPage />);

    await waitFor(() => {
      expect(screen.getByLabelText(/nombre/i)).toBeInTheDocument();
    });

    fireEvent.change(screen.getByLabelText(/nombre/i), { target: { value: 'Actualizado' } });

    fireEvent.click(screen.getByRole('button', { name: /actualizar/i }));

    // ASSERT: Verificar que el servicio reciba la solicitud de actualizacion
    await waitFor(() => {
      expect(productoService.productoService.actualizar).toHaveBeenCalledWith('test-id', expect.any(Object));
    });
  });
});
