import { http, HttpResponse } from 'msw';

/**
 * Handlers base para pruebas de integracion.
 * Se pueden extender por archivo de prueba segun cada escenario.
 */
export const handlers = [
    http.get('/__msw/health', () => HttpResponse.json({ ok: true })),
];
