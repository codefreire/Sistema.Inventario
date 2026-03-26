import axios from 'axios';

/**
 * Cliente Axios configurado para comunicación con la API
 * Base URL: Variable de entorno VITE_API_BASE_URL
 * Headers por defecto: Content-Type: application/json
 * 
 * @example
 * - Obtener datos
 * const response = await apiClient.get('/productos');
 * 
 * - Crear recurso
 * const newItem = await apiClient.post('/productos', datos);
 */
const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export default apiClient;