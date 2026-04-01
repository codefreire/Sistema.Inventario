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

/**
 * Interceptor de respuestas para extraer mensajes de error del backend.
 * Cuando el backend devuelve un error (400, 404, 500, etc.), este interceptor
 * extrae el mensaje específico de la respuesta y lo propaga en error.message
 * para que las páginas puedan mostrarlo al usuario.
 */
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.data) {
      const data = error.response.data;
      if (typeof data === 'string' && data.length > 0) {
        error.message = data;
      } else if (typeof data === 'object' && data.title) {
        error.message = data.title;
      }
    }
    return Promise.reject(error);
  }
);

export default apiClient;