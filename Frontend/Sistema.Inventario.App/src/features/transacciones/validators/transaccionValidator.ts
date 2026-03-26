import type { CrearTransaccionRequest } from '../types/Transaccion';

export interface ErroresTransaccion {
  tipoTransaccion?: string;
  productoId?: string;
  cantidad?: string;
  precioUnitario?: string;
  detalle?: string;
}

/**
 * Valida una transaccion de inventario antes de enviarla al backend.
 *
 * Casos relevantes y bordes cubiertos:
 * - El tipo solo admite `Compra` o `Venta` (no sensible a mayusculas).
 * - En ventas, la cantidad no puede superar el `stockDisponible` cuando se proporciona.
 * - Cantidad debe ser entero positivo.
 * - Precio unitario no puede ser negativo ni exceder el maximo permitido.
 * - Detalle obligatorio con longitud maxima.
 *
 * @param {CrearTransaccionRequest} transaccion Datos de la transaccion a validar.
 * @param {number} [stockDisponible] Stock actual del producto para validar ventas.
 * @returns {ErroresTransaccion} Objeto con mensajes de error por campo; vacio cuando la entrada es valida.
 * @throws {TypeError} Si `transaccion` es `null` o `undefined` en tiempo de ejecucion.
 *
 * @example
 * const errores = validarTransaccion(
 *   {
 *     tipoTransaccion: 'Venta',
 *     productoId: 'prod-001',
 *     cantidad: 3,
 *     precioUnitario: 12.5,
 *     detalle: 'Venta mostrador',
 *   },
 *   10
 * );
 */
export function validarTransaccion(
  transaccion: CrearTransaccionRequest,
  stockDisponible?: number
): ErroresTransaccion {
  const errores: ErroresTransaccion = {};

  if (!transaccion.tipoTransaccion || transaccion.tipoTransaccion.trim() === '') {
    errores.tipoTransaccion = 'El tipo de transacción es obligatorio.';
  } else if (!/^(Compra|Venta)$/i.test(transaccion.tipoTransaccion)) {
    errores.tipoTransaccion = 'El tipo debe ser "Compra" o "Venta".';
  }

  if (!transaccion.productoId || transaccion.productoId.trim() === '') {
    errores.productoId = 'Debe seleccionar un producto.';
  }

  if (transaccion.cantidad === undefined || transaccion.cantidad === null) {
    errores.cantidad = 'La cantidad es obligatoria.';
  } else if (!Number.isInteger(transaccion.cantidad)) {
    errores.cantidad = 'La cantidad debe ser un número entero.';
  } else if (transaccion.cantidad <= 0) {
    errores.cantidad = 'La cantidad debe ser mayor a 0.';
  } else if (
    transaccion.tipoTransaccion?.toLowerCase() === 'venta' &&
    stockDisponible !== undefined &&
    transaccion.cantidad > stockDisponible
  ) {
    errores.cantidad = `No puede vender más del stock disponible (${stockDisponible} unidades).`;
  }

  if (transaccion.precioUnitario === undefined || transaccion.precioUnitario === null) {
    errores.precioUnitario = 'El precio unitario es obligatorio.';
  } else if (transaccion.precioUnitario < 0) {
    errores.precioUnitario = 'El precio unitario debe ser mayor o igual a 0.';
  } else if (transaccion.precioUnitario > 99999999.99) {
    errores.precioUnitario = 'El precio unitario no puede exceder 99,999,999.99.';
  }

  if (!transaccion.detalle || transaccion.detalle.trim() === '') {
    errores.detalle = 'El detalle es obligatorio.';
  } else if (transaccion.detalle.length > 500) {
    errores.detalle = 'El detalle no puede exceder 500 caracteres.';
  }

  return errores;
}

export function tieneErrores(errores: ErroresTransaccion): boolean {
  return Object.values(errores).some((error) => error !== undefined);
}