import type { Transaccion } from '../types/Transaccion';
import { formatearMoneda } from '../../../shared/utils/formatCurrency';

interface TransaccionCardProps {
  transaccion: Transaccion;
}

export default function TransaccionCard({ transaccion }: TransaccionCardProps) {
  return (
    <div className="transaccion-card">
      <h3>{transaccion.tipoTransaccion}</h3>
      <p><strong>Fecha:</strong> {new Date(transaccion.fecha).toLocaleDateString('es-EC')}</p>
      <p><strong>Cantidad:</strong> {transaccion.cantidad}</p>
      <p><strong>P. Unitario:</strong> {formatearMoneda(transaccion.precioUnitario)}</p>
      <p><strong>Total:</strong> {formatearMoneda(transaccion.precioTotal)}</p>
      <p><strong>Detalle:</strong> {transaccion.detalle}</p>
    </div>
  );
}