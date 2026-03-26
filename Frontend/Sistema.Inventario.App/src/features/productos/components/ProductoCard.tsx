import type { Producto } from '../types/Producto';
import { formatearMoneda } from '../../../shared/utils/formatCurrency';

interface ProductoCardProps {
  producto: Producto;
}

export default function ProductoCard({ producto }: ProductoCardProps) {
  return (
    <div className="producto-card">
      <h3>{producto.nombre}</h3>
      <p>{producto.descripcion}</p>
      <p><strong>Categoría:</strong> {producto.categoria}</p>
      <p><strong>Precio:</strong> {formatearMoneda(producto.precio)}</p>
      <p><strong>Stock:</strong> {producto.stock}</p>
    </div>
  );
}