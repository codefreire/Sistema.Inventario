import '@testing-library/jest-dom/vitest';
import { cleanup } from '@testing-library/react';
import { afterAll, afterEach, beforeAll } from 'vitest';
import { servidorPruebas } from './mocks/server';

// Inicializa el servidor MSW una sola vez antes de ejecutar toda la suite.
beforeAll(() => {
    servidorPruebas.listen({ onUnhandledRequest: 'error' });
});

// Limpia el DOM y reinicia handlers personalizados despues de cada prueba.
afterEach(() => {
    cleanup();
    servidorPruebas.resetHandlers();
});

// Cierra el servidor MSW al finalizar toda la ejecucion de pruebas.
afterAll(() => {
    servidorPruebas.close();
});
