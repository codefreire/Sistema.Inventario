import { fireEvent, screen, waitFor } from '@testing-library/react';
import { describe, expect, it, vi, beforeEach } from 'vitest';
import CrearProductoPage from '../../../../features/productos/pages/CrearProductoPage';
import * as productoService from '../../../../features/productos/services/productoService';
import { renderConProveedores } from '../../../setup/testUtils';

/**
 * Mock del servicio de productos para evitar llamadas reales durante la prueba.
 */
vi.mock('../../../../features/productos/services/productoService');

const mockNavigate = vi.fn();

/**
 * Mock de react-router-dom para controlar la navegacion desde la pagina.
 */
vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom');
  return {
    ...actual,
    useNavigate: () => mockNavigate,
  };
});

/**
 * Pruebas de integracion para la pagina CrearProductoPage.
 */
describe('CrearProductoPage', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockNavigate.mockClear();
  });

  /**
   * Verifica que el formulario principal de creacion se renderice completo.
   */
  it('DebeRenderizarFormularioCompleto', () => {
    // ACT: Cargar el formulario principal en pantalla
    renderConProveedores(<CrearProductoPage />);

    // ASSERT: Verificar que los campos principales del formulario esten disponibles
    expect(screen.getByRole('heading', { name: /crear producto/i })).toBeInTheDocument();
    expect(screen.getByLabelText(/nombre/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/descripción/i)).toBeInTheDocument();
  });

  /**
   * Verifica que al cancelar se navegue nuevamente al listado de productos.
   */
  it('AlCancelarDebeNavegar', () => {
    // ACT: Presionar cancelar desde el formulario de creacion
    renderConProveedores(<CrearProductoPage />);
    const botonCancelar = screen.getByRole('button', { name: /cancelar/i });
    fireEvent.click(botonCancelar);

    // ASSERT: Verificar que se navegue al listado de productos
    expect(mockNavigate).toHaveBeenCalledWith('/productos');
  });

  /**
   * Verifica que al enviar un producto valido se invoque el servicio de creacion.
   */
  it('AlSubmitConImagenDebeCallService', async () => {
    // ARRANGE: Configurar el mock de creacion y preparar un formulario valido
    vi.mocked(productoService.productoService.crear).mockResolvedValueOnce({
      id: '1',
      nombre: 'Test',
      descripcion: 'Test',
      categoria: 'Test',
      imagenUrl: 'https://test.com/test.jpg',
      precio: 100,
      stock: 1,
    });

    // ACT: Completar el formulario y enviarlo con datos validos
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

    // ASSERT: Verificar que el servicio de creacion sea invocado
    await waitFor(
      () => {
        expect(productoService.productoService.crear).toHaveBeenCalled();
      },
      { timeout: 3000 }
    );
  });
});
