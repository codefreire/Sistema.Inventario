export interface Transaccion {
  id: string;
  fecha: string;
  tipoTransaccion: string;
  productoId: string;
  cantidad: number;
  precioUnitario: number;
  precioTotal: number;
  detalle: string;
}

export interface CrearTransaccionRequest {
  tipoTransaccion: string;
  productoId: string;
  cantidad: number;
  precioUnitario: number;
  detalle: string;
}

export interface ActualizarTransaccionRequest {
  tipoTransaccion: string;
  productoId: string;
  cantidad: number;
  precioUnitario: number;
  detalle: string;
}

export interface FiltrosTransaccion {
  tipoTransaccion?: string;
  productoId?: string;
  fechaDesde?: string;
  fechaHasta?: string;
  cantidadMin?: number;
  cantidadMax?: number;
}