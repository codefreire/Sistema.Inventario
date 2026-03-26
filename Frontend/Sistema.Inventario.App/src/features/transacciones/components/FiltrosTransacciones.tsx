import type { FiltrosTransaccion } from '../types/Transaccion';
import type { Producto } from '../../productos/types/Producto';
import Button from '../../../shared/components/Button';

interface FiltrosTransaccionesProps {
  filtros: FiltrosTransaccion;
  onCambiarFiltros: (filtros: FiltrosTransaccion) => void;
  productos: Producto[];
}

export default function FiltrosTransacciones({ filtros, onCambiarFiltros, productos }: FiltrosTransaccionesProps) {
  const handleCambio = (campo: keyof FiltrosTransaccion, valor: string) => {
    const nuevosFiltros = { ...filtros };
    if (valor === '') {
      delete nuevosFiltros[campo];
    } else if (['cantidadMin', 'cantidadMax'].includes(campo)) {
      (nuevosFiltros as Record<string, unknown>)[campo] = Number(valor);
    } else {
      (nuevosFiltros as Record<string, unknown>)[campo] = valor;
    }
    onCambiarFiltros(nuevosFiltros);
  };

  const limpiarFiltros = () => onCambiarFiltros({});

  return (
    <div className="filtros-panel">
      <h3>Filtros Avanzados</h3>
      <div className="filtros-grid">
        <div className="campo">
          <label>Tipo de Transacción</label>
          <select value={filtros.tipoTransaccion ?? ''} onChange={(e) => handleCambio('tipoTransaccion', e.target.value)}>
            <option value="">Todos</option>
            <option value="Compra">Compra</option>
            <option value="Venta">Venta</option>
          </select>
        </div>
        <div className="campo">
          <label>Producto</label>
          <select value={filtros.productoId ?? ''} onChange={(e) => handleCambio('productoId', e.target.value)}>
            <option value="">Todos</option>
            {productos.map((p) => (
              <option key={p.id} value={p.id}>{p.nombre}</option>
            ))}
          </select>
        </div>
        <div className="campo">
          <label>Fecha Desde</label>
          <input type="date" value={filtros.fechaDesde ?? ''} onChange={(e) => handleCambio('fechaDesde', e.target.value)} />
        </div>
        <div className="campo">
          <label>Fecha Hasta</label>
          <input type="date" value={filtros.fechaHasta ?? ''} onChange={(e) => handleCambio('fechaHasta', e.target.value)} />
        </div>
        <div className="campo">
          <label>Cantidad Mínima</label>
          <input type="number" placeholder="0" min="0" value={filtros.cantidadMin ?? ''} onChange={(e) => handleCambio('cantidadMin', e.target.value)} />
        </div>
        <div className="campo">
          <label>Cantidad Máxima</label>
          <input type="number" placeholder="Sin límite" min="0" value={filtros.cantidadMax ?? ''} onChange={(e) => handleCambio('cantidadMax', e.target.value)} />
        </div>
      </div>
      <div className="filtros-acciones">
        <Button variante="secundario" onClick={limpiarFiltros}>Limpiar Filtros</Button>
      </div>
    </div>
  );
}
