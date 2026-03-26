import { setupServer } from 'msw/node';
import { handlers } from './handlers';

/**
 * Servidor MSW para interceptar llamadas HTTP durante las pruebas.
 */
export const servidorPruebas = setupServer(...handlers);
