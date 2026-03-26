import '@testing-library/jest-dom/vitest';
import { cleanup } from '@testing-library/react';
import { afterAll, afterEach, beforeAll } from 'vitest';
import { servidorPruebas } from './mocks/server';

beforeAll(() => {
    servidorPruebas.listen({ onUnhandledRequest: 'error' });
});

afterEach(() => {
    cleanup();
    servidorPruebas.resetHandlers();
});

afterAll(() => {
    servidorPruebas.close();
});
