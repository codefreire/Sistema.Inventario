import apiClient from '../../../shared/services/apiClient';
import type { Transaccion, CrearTransaccionRequest, ActualizarTransaccionRequest } from '../types/Transaccion';

const RUTA = '/transacciones';

/**
 * Servicio de acceso a datos para la entidad Transaccion.
 *
 * Proporciona operaciones CRUD sobre el endpoint de transacciones
 * utilizando `apiClient` para la comunicación con la API.
 */
export const transaccionService = {
  /**
   * Obtiene el listado completo de transacciones.
   *
   * @returns {Promise<Transaccion[]>} Promesa con la colección de transacciones registradas.
   * @throws {Error} Lanza un error cuando la solicitud HTTP falla o el servidor responde con error.
   */
  async obtenerTodas(): Promise<Transaccion[]> {
    const respuesta = await apiClient.get<Transaccion[]>(RUTA);
    return respuesta.data;
  },

  /**
   * Obtiene una transacción específica por su identificador.
   *
   * @param {string} id Identificador único de la transacción a consultar.
   * @returns {Promise<Transaccion>} Promesa con la transacción encontrada.
   * @throws {Error} Lanza un error cuando la transacción no existe o la solicitud HTTP falla.
   */
  async obtenerPorId(id: string): Promise<Transaccion> {
    const respuesta = await apiClient.get<Transaccion>(`${RUTA}/${id}`);
    return respuesta.data;
  },

  /**
   * Crea una nueva transacción con la información proporcionada.
   *
   * @param {CrearTransaccionRequest} transaccion Datos requeridos para crear la transacción.
   * @returns {Promise<Transaccion>} Promesa con la transacción creada por el backend.
   * @throws {Error} Lanza un error cuando los datos son inválidos o la solicitud HTTP falla.
   */
  async crear(transaccion: CrearTransaccionRequest): Promise<Transaccion> {
    const respuesta = await apiClient.post<Transaccion>(RUTA, transaccion);
    return respuesta.data;
  },

  /**
   * Actualiza la información de una transacción existente.
   *
   * @param {string} id Identificador único de la transacción a actualizar.
   * @param {ActualizarTransaccionRequest} transaccion Datos de actualización de la transacción.
   * @returns {Promise<Transaccion>} Promesa con la transacción actualizada.
   * @throws {Error} Lanza un error cuando la transacción no existe, los datos son inválidos o la solicitud HTTP falla.
   */
  async actualizar(id: string, transaccion: ActualizarTransaccionRequest): Promise<Transaccion> {
    const respuesta = await apiClient.put<Transaccion>(`${RUTA}/${id}`, transaccion);
    return respuesta.data;
  },

  /**
   * Elimina una transacción por su identificador.
   *
   * @param {string} id Identificador único de la transacción a eliminar.
   * @returns {Promise<void>} Promesa resuelta cuando la eliminación se completa correctamente.
   * @throws {Error} Lanza un error cuando la transacción no existe o la solicitud HTTP falla.
   */
  async eliminar(id: string): Promise<void> {
    await apiClient.delete(`${RUTA}/${id}`);
  },
};