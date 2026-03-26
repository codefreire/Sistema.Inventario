import apiClient from '../../../shared/services/apiClient';
import type { Producto, CrearProductoRequest, ActualizarProductoRequest } from '../types/Producto';

const RUTA = '/productos';

export const productoService = {
  async obtenerTodos(): Promise<Producto[]> {
    const respuesta = await apiClient.get<Producto[]>(RUTA);
    return respuesta.data;
  },

  async obtenerPorId(id: string): Promise<Producto> {
    const respuesta = await apiClient.get<Producto>(`${RUTA}/${id}`);
    return respuesta.data;
  },

  async crear(producto: CrearProductoRequest): Promise<Producto> {
    const respuesta = await apiClient.post<Producto>(RUTA, producto);
    return respuesta.data;
  },

  async actualizar(id: string, producto: ActualizarProductoRequest): Promise<Producto> {
    const respuesta = await apiClient.put<Producto>(`${RUTA}/${id}`, producto);
    return respuesta.data;
  },

  async eliminar(id: string): Promise<void> {
    await apiClient.delete(`${RUTA}/${id}`);
  },
};