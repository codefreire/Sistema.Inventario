import { fireEvent, screen, waitFor } from '@testing-library/react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import ConsultarProductoPage from '../../../../features/productos/pages/ConsultarProductoPage';
import * as productoService from '../../../../features/productos/services/productoService';
import { renderConProveedores } from '../../../setup/testUtils';

/**
 * Mock del servicio de productos para controlar respuestas de consulta.
 */
vi.mock('../../../../features/productos/services/productoService');

const mockNavigate = vi.fn();
const mockParams = { id: 'prod-123' };

/**
 * Mock de react-router-dom para simular parametros y navegacion.
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
 * Pruebas de integracion para la pagina ConsultarProductoPage.
 */
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

  /**
   * Verifica que se carguen y muestren los datos del producto consultado.
   */
  it('DebeCargarYMostrarDatosDelProducto', async () => {
    // ARRANGE: Renderizar la pagina con un producto configurado en el mock del servicio
    renderConProveedores(<ConsultarProductoPage />);

    // ASSERT: Verificar que se consulten y muestren los datos esperados del producto
    await waitFor(() => {
      expect(productoService.productoService.obtenerPorId).toHaveBeenCalledWith('prod-123');
      expect(screen.getByDisplayValue('Mesa')).toBeInTheDocument();
      expect(screen.getByDisplayValue('Mesa rectangular de madera')).toBeInTheDocument();
      expect(screen.getByDisplayValue('Hogar')).toBeInTheDocument();
      expect(screen.getByDisplayValue('5')).toBeInTheDocument();
      expect(screen.getByRole('heading', { name: /consulta de producto/i })).toBeInTheDocument();
    });
  });

  /**
   * Verifica que el boton de retorno navegue al listado de productos.
   */
  it('VolverAProductos_CuandoSePresionaBoton_DebeNavegarAListado', async () => {
    // ARRANGE: Renderizar la pagina y esperar a que el boton de retorno este disponible
    renderConProveedores(<ConsultarProductoPage />);

    await waitFor(() => {
      expect(screen.getByRole('button', { name: /volver a productos/i })).toBeInTheDocument();
    });

    // ACT: Hacer clic en el boton para volver al listado
    fireEvent.click(screen.getByRole('button', { name: /volver a productos/i }));

    // ASSERT: Verificar que se navegue a la ruta principal de productos
    expect(mockNavigate).toHaveBeenCalledWith('/productos');
  });

  /**
   * Verifica que el boton de edicion navegue al formulario de actualizacion.
   */
  it('IrAEdicion_CuandoSePresionaBoton_DebeNavegarAEditarProducto', async () => {
    // ARRANGE: Renderizar la pagina y esperar a que el boton de edicion este visible
    renderConProveedores(<ConsultarProductoPage />);

    await waitFor(() => {
      expect(screen.getByRole('button', { name: /ir a edición/i })).toBeInTheDocument();
    });

    // ACT: Hacer clic en el boton para ir al formulario de edicion
    fireEvent.click(screen.getByRole('button', { name: /ir a edición/i }));

    // ASSERT: Verificar que la navegacion apunte a la ruta de edicion del producto
    expect(mockNavigate).toHaveBeenCalledWith('/productos/editar/prod-123');
  });

  /**
   * Verifica que cuando la consulta falle se muestre una notificacion de error.
   */
  it('ErrorAlConsultar_CuandoServicioFalla_DebeMostrarNotificacion', async () => {
    // ARRANGE: Configurar el servicio para que falle al consultar el producto
    vi.mocked(productoService.productoService.obtenerPorId).mockRejectedValueOnce(new Error('Producto no encontrado'));

    // ACT: Renderizar la pagina con el escenario de error configurado
    renderConProveedores(<ConsultarProductoPage />);

    // ASSERT: Verificar que se informe el error y permanezca disponible la accion de retorno
    await waitFor(() => {
      expect(screen.getByText(/producto no encontrado/i)).toBeInTheDocument();
      expect(screen.getByRole('button', { name: /volver a productos/i })).toBeInTheDocument();
    });
  });
});
