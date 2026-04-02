import { screen, waitFor } from '@testing-library/react';
import { describe, expect, it, vi, beforeEach } from 'vitest';
import ProductosPage from '../../../../features/productos/pages/ProductosPage';
import * as productoService from '../../../../features/productos/services/productoService';
import { renderConProveedores } from '../../../setup/testUtils';

/**
 * Mock del servicio de productos para desacoplar las pruebas de la API real.
 */
vi.mock('../../../../features/productos/services/productoService');

const mockNavigate = vi.fn();

/**
 * Mock de react-router-dom para controlar la navegacion durante las pruebas.
 */
vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom');
  return {
    ...actual,
    useNavigate: () => mockNavigate,
  };
});

/**
 * Pruebas de integracion para la pagina ProductosPage.
 */
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

  /**
   * Verifica que la pagina cargue productos al inicializarse.
   */
  it('DebeCargarProductos', async () => {
    // ACT: Esperar la carga inicial de productos desde el componente
    renderConProveedores(<ProductosPage />);

    // ASSERT: Verificar que se consulte el servicio y se muestre el producto esperado
    await waitFor(() => {
      expect(productoService.productoService.obtenerTodos).toHaveBeenCalled();
      expect(screen.getByText('Laptop')).toBeInTheDocument();
    });
  });

  /**
   * Verifica que la tabla renderice los datos principales del producto.
   */
  it('DebeRenderizarTabla', async () => {
    // ACT: Esperar a que la tabla termine de renderizar la informacion
    renderConProveedores(<ProductosPage />);

    // ASSERT: Verificar que la tabla muestre nombre y categoria del producto
    await waitFor(() => {
      expect(screen.getByText('Laptop')).toBeInTheDocument();
      expect(screen.getByText('Tech')).toBeInTheDocument();
    });
  });

  /**
   * Verifica que la pagina soporte el escenario sin productos disponibles.
   */
  it('DebeRenderizarSinProductos', async () => {
    // ARRANGE: Configurar el servicio para responder con una coleccion vacia
    vi.mocked(productoService.productoService.obtenerTodos).mockResolvedValueOnce([]);

    // ACT: Renderizar la pagina con el escenario sin datos
    renderConProveedores(<ProductosPage />);

    // ASSERT: Verificar que la consulta al servicio se ejecute correctamente
    await waitFor(() => {
      expect(productoService.productoService.obtenerTodos).toHaveBeenCalled();
    });
  });
});
