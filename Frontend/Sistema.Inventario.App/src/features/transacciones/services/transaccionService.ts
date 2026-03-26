import apiClient from '../../../shared/services/apiClient';
import type { Transaccion, CrearTransaccionRequest, ActualizarTransaccionRequest } from '../types/Transaccion';

const RUTA = '/transacciones';

export const transaccionService = {
  async obtenerTodas(): Promise<Transaccion[]> {
    const respuesta = await apiClient.get<Transaccion[]>(RUTA);
    return respuesta.data;
  },

  async obtenerPorId(id: string): Promise<Transaccion> {
    const respuesta = await apiClient.get<Transaccion>(`${RUTA}/${id}`);
    return respuesta.data;
  },

  async crear(transaccion: CrearTransaccionRequest): Promise<Transaccion> {
    const respuesta = await apiClient.post<Transaccion>(RUTA, transaccion);
    return respuesta.data;
  },

  async actualizar(id: string, transaccion: ActualizarTransaccionRequest): Promise<Transaccion> {
    const respuesta = await apiClient.put<Transaccion>(`${RUTA}/${id}`, transaccion);
    return respuesta.data;
  },

  async eliminar(id: string): Promise<void> {
    await apiClient.delete(`${RUTA}/${id}`);
  },
};