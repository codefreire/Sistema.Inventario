import type { FiltrosProducto } from '../types/Producto';
import Button from '../../../shared/components/Button';

interface FiltrosProductosProps {
  filtros: FiltrosProducto;
  onCambiarFiltros: (filtros: FiltrosProducto) => void;
}

export default function FiltrosProductos({ filtros, onCambiarFiltros }: FiltrosProductosProps) {
  const handleCambio = (campo: keyof FiltrosProducto, valor: string) => {
    const nuevosFiltros = { ...filtros };
    if (valor === '') {
      delete nuevosFiltros[campo];
    } else if (['precioMin', 'precioMax', 'stockMin', 'stockMax'].includes(campo)) {
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
          <label>Nombre</label>
          <input
            type="text"
            placeholder="Buscar por nombre..."
            value={filtros.nombre ?? ''}
            onChange={(e) => handleCambio('nombre', e.target.value)}
          />
        </div>
        <div className="campo">
          <label>Categoría</label>
          <input
            type="text"
            placeholder="Buscar por categoría..."
            value={filtros.categoria ?? ''}
            onChange={(e) => handleCambio('categoria', e.target.value)}
          />
        </div>
        <div className="campo">
          <label>Precio Mínimo</label>
          <input
            type="number"
            placeholder="0.00"
            min="0"
            step="0.01"
            value={filtros.precioMin ?? ''}
            onChange={(e) => handleCambio('precioMin', e.target.value)}
          />
        </div>
        <div className="campo">
          <label>Precio Máximo</label>
          <input
            type="number"
            placeholder="99999999.99"
            min="0"
            step="0.01"
            value={filtros.precioMax ?? ''}
            onChange={(e) => handleCambio('precioMax', e.target.value)}
          />
        </div>
        <div className="campo">
          <label>Stock Mínimo</label>
          <input
            type="number"
            placeholder="0"
            min="0"
            value={filtros.stockMin ?? ''}
            onChange={(e) => handleCambio('stockMin', e.target.value)}
          />
        </div>
        <div className="campo">
          <label>Stock Máximo</label>
          <input
            type="number"
            placeholder="Sin límite"
            min="0"
            value={filtros.stockMax ?? ''}
            onChange={(e) => handleCambio('stockMax', e.target.value)}
          />
        </div>
      </div>
      <div className="filtros-acciones">
        <Button variante="secundario" onClick={limpiarFiltros}>Limpiar Filtros</Button>
      </div>
    </div>
  );
}
