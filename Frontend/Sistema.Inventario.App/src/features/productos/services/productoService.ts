import apiClient from '../../../shared/services/apiClient';
import type { Producto, CrearProductoRequest, ActualizarProductoRequest } from '../types/Producto';

const RUTA = '/productos';

/**
 * Servicio de acceso a datos para la entidad Producto.
 *
 * Centraliza las operaciones CRUD contra el endpoint de productos,
 * delegando la comunicación HTTP en el cliente `apiClient`.
 */
export const productoService = {
  /**
   * Obtiene el listado completo de productos.
   *
   * @returns {Promise<Producto[]>} Promesa con la colección de productos registrados.
   * @throws {Error} Lanza un error cuando la solicitud HTTP falla o el servidor responde con error.
   */
  async obtenerTodos(): Promise<Producto[]> {
    const respuesta = await apiClient.get<Producto[]>(RUTA);
    return respuesta.data;
  },

  /**
   * Obtiene un producto específico a partir de su identificador.
   *
   * @param {string} id Identificador único del producto a consultar.
   * @returns {Promise<Producto>} Promesa con el producto encontrado.
   * @throws {Error} Lanza un error cuando el producto no existe o la solicitud HTTP falla.
   */
  async obtenerPorId(id: string): Promise<Producto> {
    const respuesta = await apiClient.get<Producto>(`${RUTA}/${id}`);
    return respuesta.data;
  },

  /**
   * Crea un nuevo producto con la información proporcionada.
   *
   * @param {CrearProductoRequest} producto Datos requeridos para crear el producto.
   * @returns {Promise<Producto>} Promesa con el producto creado por el backend.
   * @throws {Error} Lanza un error cuando los datos son inválidos o la solicitud HTTP falla.
   */
  async crear(producto: CrearProductoRequest): Promise<Producto> {
    const respuesta = await apiClient.post<Producto>(RUTA, producto);
    return respuesta.data;
  },

  /**
   * Actualiza la información de un producto existente.
   *
   * @param {string} id Identificador único del producto a actualizar.
   * @param {ActualizarProductoRequest} producto Datos de actualización del producto.
   * @returns {Promise<Producto>} Promesa con el producto actualizado.
   * @throws {Error} Lanza un error cuando el producto no existe, los datos son inválidos o la solicitud HTTP falla.
   */
  async actualizar(id: string, producto: ActualizarProductoRequest): Promise<Producto> {
    const respuesta = await apiClient.put<Producto>(`${RUTA}/${id}`, producto);
    return respuesta.data;
  },

  /**
   * Elimina un producto por su identificador.
   *
   * @param {string} id Identificador único del producto a eliminar.
   * @returns {Promise<void>} Promesa resuelta cuando la eliminación se completa correctamente.
   * @throws {Error} Lanza un error cuando el producto no existe o la solicitud HTTP falla.
   */
  async eliminar(id: string): Promise<void> {
    await apiClient.delete(`${RUTA}/${id}`);
  },
};