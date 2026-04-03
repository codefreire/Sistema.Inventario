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

/**
 * Pruebas unitarias del servicio productoService.
 */
describe('productoService', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  /**
   * Agrupa los escenarios de consulta de productos.
   */
  describe('obtenerTodos', () => {
    /**
     * Verifica que obtenerTodos retorna una lista de productos mockeados correctamente.
     */
    it('ObtenerTodos_CuandoLaApiRetornaProductos_DebeRetornarLaLista', async () => {
      // ARRANGE: Preparar una respuesta simulada de la API con dos productos
      const productosEsperados = [
        { id: '1', nombre: 'Mouse', descripcion: 'Mouse inalambrico', categoria: 'Perifericos', imagenUrl: 'https://cdn.com/mouse.jpg', precio: 25.99, stock: 5 },
        { id: '2', nombre: 'Teclado', descripcion: 'Teclado mecanico', categoria: 'Perifericos', imagenUrl: 'https://cdn.com/teclado.jpg', precio: 99.99, stock: 3 },
      ];

      vi.mocked(apiClient.get).mockResolvedValueOnce({ data: productosEsperados });

      // ACT: Ejecutar la consulta de productos a traves del servicio
      const resultado = await productoService.obtenerTodos();

      // ASSERT: Verificar que el servicio retorne la lista y consulte el endpoint correcto
      expect(resultado).toEqual(productosEsperados);
      expect(apiClient.get).toHaveBeenCalledWith('/productos');
    });

    /**
     * Verifica que obtenerTodos lanza error cuando la API falla.
     */
    it('ObtenerTodos_CuandoLaApiLanzaError_DebePropagaElError', async () => {
      // ARRANGE: Configurar el cliente HTTP para lanzar un error de conexion
      const errorEsperado = new Error('Error de conexión');
      vi.mocked(apiClient.get).mockRejectedValueOnce(errorEsperado);

      // ACT & ASSERT: Ejecutar la consulta y verificar que el error se propague
      await expect(productoService.obtenerTodos()).rejects.toThrow('Error de conexión');
    });
  });

  /**
   * Agrupa los escenarios de carga de imagenes de productos.
   */
  describe('subirImagen', () => {
    /**
     * Verifica que subirImagen envia FormData y retorna la URL publica.
     */
    it('SubirImagen_CuandoArchivoEsValido_DebeRetornarUrlPublica', async () => {
      // ARRANGE: Preparar un archivo valido y una respuesta exitosa del backend
      const archivoTest = new File(['contenido'], 'test.jpg', { type: 'image/jpeg' });
      const urlEsperada = 'http://localhost:5261/imagenes/uuid-123.jpg';

      vi.mocked(apiClient.post).mockResolvedValueOnce({
        data: { imagenUrl: urlEsperada } as SubirImagenResponse,
      });

      // ACT: Ejecutar la carga de imagen a traves del servicio
      const resultado = await productoService.subirImagen(archivoTest);

      // ASSERT: Verificar que se retorne la URL y se use multipart/form-data
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
      // ARRANGE: Preparar un archivo valido y un mock de respuesta satisfactoria
      const archivoTest = new File(['data'], 'imagen.png', { type: 'image/png' });

      vi.mocked(apiClient.post).mockResolvedValueOnce({
        data: { imagenUrl: 'http://localhost:5261/imagenes/test.png' },
      });

      // ACT: Ejecutar la carga de imagen para inspeccionar el payload enviado
      await productoService.subirImagen(archivoTest);

      // ASSERT: Verificar que el segundo argumento enviado sea una instancia de FormData
      const llamada = vi.mocked(apiClient.post).mock.calls[0];
      const formData = llamada[1] as FormData;
      expect(formData).toBeInstanceOf(FormData);
    });

    /**
     * Verifica que subirImagen lanza error cuando el backend rechaza el archivo.
     */
    it('SubirImagen_CuandoBackendRechazaArchivo_DebeLanzarError', async () => {
      // ARRANGE: Preparar un archivo invalido y configurar rechazo desde el backend
      const archivoInvalido = new File(['data'], 'documento.pdf', { type: 'application/pdf' });
      const errorEsperado = new Error('La extension del archivo no esta permitida');

      vi.mocked(apiClient.post).mockRejectedValueOnce(errorEsperado);

      // ACT & ASSERT: Ejecutar la carga y verificar que el error esperado se propague
      await expect(productoService.subirImagen(archivoInvalido)).rejects.toThrow(
        'La extension del archivo no esta permitida'
      );
    });
  });

  /**
   * Agrupa los escenarios de creacion de productos.
   */
  describe('crear', () => {
    /**
     * Verifica que crear envia el payload correcto y retorna el producto creado.
     */
    it('Crear_CuandoDatosValidos_DebeRetornarProductoCreado', async () => {
      // ARRANGE: Preparar un request valido y la respuesta simulada del endpoint de creacion
      const request: CrearProductoRequest = {
        nombre: 'Monitor 27',
        descripcion: 'Monitor para oficina',
        categoria: 'Tecnologia',
        imagenUrl: 'http://localhost:5261/imagenes/uuid-123.jpg',
        precio: 450.00,
        stock: 2,
      };

      const productoCreado = {
        id: 'guid-123',
        ...request,
      };

      vi.mocked(apiClient.post).mockResolvedValueOnce({ data: productoCreado });

      // ACT: Ejecutar la creacion del producto desde el servicio
      const resultado = await productoService.crear(request);

      // ASSERT: Verificar que se retorne el producto creado y se use el endpoint correcto
      expect(resultado).toEqual(productoCreado);
      expect(apiClient.post).toHaveBeenCalledWith('/productos', request);
    });
  });

  /**
   * Agrupa los escenarios de actualizacion de productos.
   */
  describe('actualizar', () => {
    /**
     * Verifica que actualizar envia PUT con los datos correctos.
     */
    it('Actualizar_CuandoProductoExiste_DebeActualizarYRetornar', async () => {
      // ARRANGE: Preparar un identificador valido y los datos de actualizacion
      const id = 'guid-456';
      const actualizacion = {
        nombre: 'Monitor 27 Enhanced',
        descripcion: 'Monitor actualizado',
        categoria: 'Tecnologia',
        imagenUrl: 'http://localhost:5261/imagenes/new-uuid.jpg',
        precio: 500.00,
        stock: 5,
      };

      const productoActualizado = {
        id,
        ...actualizacion,
      };

      vi.mocked(apiClient.put).mockResolvedValueOnce({ data: productoActualizado });

      // ACT: Ejecutar la actualizacion del producto a traves del servicio
      const resultado = await productoService.actualizar(id, actualizacion);

      // ASSERT: Verificar que se retorne el producto actualizado y se use la ruta esperada
      expect(resultado).toEqual(productoActualizado);
      expect(apiClient.put).toHaveBeenCalledWith(`/productos/${id}`, actualizacion);
    });
  });

  /**
   * Agrupa los escenarios de eliminacion de productos.
   */
  describe('eliminar', () => {
    /**
     * Verifica que eliminar realiza DEL correctamente.
     */
    it('Eliminar_CuandoProductoExiste_DebeEliminarSinError', async () => {
      // ARRANGE: Preparar un identificador valido y una respuesta exitosa del delete
      const id = 'guid-789';
      vi.mocked(apiClient.delete).mockResolvedValueOnce({});

      // ACT: Ejecutar la eliminacion del producto
      await productoService.eliminar(id);

      // ASSERT: Verificar que se invoque el endpoint delete correspondiente
      expect(apiClient.delete).toHaveBeenCalledWith(`/productos/${id}`);
    });
  });
});
