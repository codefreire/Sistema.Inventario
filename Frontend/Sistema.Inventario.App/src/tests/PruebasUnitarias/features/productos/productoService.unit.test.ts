import { describe, expect, it, vi, beforeEach } from 'vitest';
import { productoService } from '../../../../features/productos/services/productoService';
import apiClient from '../../../../shared/services/apiClient';
import type { CrearProductoRequest, SubirImagenResponse } from '../../../../features/productos/types/Producto';

/**
 * Mock del apiClient para pruebas sin hacer solicitudes reales.
 */
vi.mock('../../../../shared/services/apiClient', () => ({
  default: {
    get: vi.fn(),
    post: vi.fn(),
    put: vi.fn(),
    delete: vi.fn(),
  },
}));

describe('productoService', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe('obtenerTodos', () => {
    /**
     * Verifica que obtenerTodos retorna una lista de productosmokeados correctamente.
     */
    it('ObtenerTodos_CuandoLaApiRetornaProductos_DebeRetornarLaLista', async () => {
      // ARRANGE:
      const productosEsperados = [
        { id: '1', nombre: 'Mouse', descripcion: 'Mouse inalambrico', categoria: 'Perifericos', imagenUrl: 'https://cdn.com/mouse.jpg', precio: 25.99, stock: 5 },
        { id: '2', nombre: 'Teclado', descripcion: 'Teclado mecanico', categoria: 'Perifericos', imagenUrl: 'https://cdn.com/teclado.jpg', precio: 99.99, stock: 3 },
      ];

      vi.mocked(apiClient.get).mockResolvedValueOnce({ data: productosEsperados });

      // ACT:
      const resultado = await productoService.obtenerTodos();

      // ASSERT:
      expect(resultado).toEqual(productosEsperados);
      expect(apiClient.get).toHaveBeenCalledWith('/productos');
    });

    /**
     * Verifica que obtenerTodos lanza error cuando la API falla.
     */
    it('ObtenerTodos_CuandoLaApiLanzaError_DebePropagaElError', async () => {
      // ARRANGE:
      const errorEsperado = new Error('Error de conexión');
      vi.mocked(apiClient.get).mockRejectedValueOnce(errorEsperado);

      // ACT & ASSERT:
      await expect(productoService.obtenerTodos()).rejects.toThrow('Error de conexión');
    });
  });

  describe('subirImagen', () => {
    /**
     * Verifica que subirImagen envia FormData y retorna la URL publica.
     */
    it('SubirImagen_CuandoArchivoEsValido_DebeRetornarUrlPublica', async () => {
      // ARRANGE:
      const archivoTest = new File(['contenido'], 'test.jpg', { type: 'image/jpeg' });
      const urlEsperada = 'http://localhost:5261/uploads/uuid-123.jpg';

      vi.mocked(apiClient.post).mockResolvedValueOnce({
        data: { imagenUrl: urlEsperada } as SubirImagenResponse,
      });

      // ACT:
      const resultado = await productoService.subirImagen(archivoTest);

      // ASSERT:
      expect(resultado).toBe(urlEsperada);
      expect(apiClient.post).toHaveBeenCalledWith(
        '/productos/imagenes',
        expect.any(FormData),
        expect.objectContaining({ headers: { 'Content-Type': 'multipart/form-data' } })
      );
    });

    /**
     * Verifica que subirImagen usa FormData con campo 'archivo'.
     */
    it('SubirImagen_DebeEnviarFormDataConCampoArchivo', async () => {
      // ARRANGE:
      const archivoTest = new File(['data'], 'imagen.png', { type: 'image/png' });

      vi.mocked(apiClient.post).mockResolvedValueOnce({
        data: { imagenUrl: 'http://localhost:5261/uploads/test.png' },
      });

      // ACT:
      await productoService.subirImagen(archivoTest);

      // ASSERT: Verificar que se llamó con FormData
      const llamada = vi.mocked(apiClient.post).mock.calls[0];
      const formData = llamada[1] as FormData;
      expect(formData).toBeInstanceOf(FormData);
    });

    /**
     * Verifica que subirImagen lanza error cuando el backend rechaza el archivo.
     */
    it('SubirImagen_CuandoBackendRechazaArchivo_DebeLanzarError', async () => {
      // ARRANGE:
      const archivoInvalido = new File(['data'], 'documento.pdf', { type: 'application/pdf' });
      const errorEsperado = new Error('La extension del archivo no esta permitida');

      vi.mocked(apiClient.post).mockRejectedValueOnce(errorEsperado);

      // ACT & ASSERT:
      await expect(productoService.subirImagen(archivoInvalido)).rejects.toThrow(
        'La extension del archivo no esta permitida'
      );
    });
  });

  describe('crear', () => {
    /**
     * Verifica que crear envia el payload correcto y retorna el producto creado.
     */
    it('Crear_CuandoDatosValidos_DebeRetornarProductoCreado', async () => {
      // ARRANGE:
      const request: CrearProductoRequest = {
        nombre: 'Monitor 27',
        descripcion: 'Monitor para oficina',
        categoria: 'Tecnologia',
        imagenUrl: 'http://localhost:5261/uploads/uuid-123.jpg',
        precio: 450.00,
        stock: 2,
      };

      const productoCreado = {
        id: 'guid-123',
        ...request,
      };

      vi.mocked(apiClient.post).mockResolvedValueOnce({ data: productoCreado });

      // ACT:
      const resultado = await productoService.crear(request);

      // ASSERT:
      expect(resultado).toEqual(productoCreado);
      expect(apiClient.post).toHaveBeenCalledWith('/productos', request);
    });
  });

  describe('actualizar', () => {
    /**
     * Verifica que actualizar envia PUT con los datos correctos.
     */
    it('Actualizar_CuandoProductoExiste_DebeActualizarYRetornar', async () => {
      // ARRANGE:
      const id = 'guid-456';
      const actualizacion = {
        nombre: 'Monitor 27 Enhanced',
        descripcion: 'Monitor actualizado',
        categoria: 'Tecnologia',
        imagenUrl: 'http://localhost:5261/uploads/new-uuid.jpg',
        precio: 500.00,
        stock: 5,
      };

      const productoActualizado = {
        id,
        ...actualizacion,
      };

      vi.mocked(apiClient.put).mockResolvedValueOnce({ data: productoActualizado });

      // ACT:
      const resultado = await productoService.actualizar(id, actualizacion);

      // ASSERT:
      expect(resultado).toEqual(productoActualizado);
      expect(apiClient.put).toHaveBeenCalledWith(`/productos/${id}`, actualizacion);
    });
  });

  describe('eliminar', () => {
    /**
     * Verifica que eliminar realiza DEL correctamente.
     */
    it('Eliminar_CuandoProductoExiste_DebeEliminarSinError', async () => {
      // ARRANGE:
      const id = 'guid-789';
      vi.mocked(apiClient.delete).mockResolvedValueOnce({});

      // ACT:
      await productoService.eliminar(id);

      // ASSERT:
      expect(apiClient.delete).toHaveBeenCalledWith(`/productos/${id}`);
    });
  });
});
