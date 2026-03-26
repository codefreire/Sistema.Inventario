import type { ReactNode } from 'react';

export interface ColumnaTabla<T> {
  encabezado: string;
  accesor: keyof T | ((item: T) => ReactNode);
  ancho?: string;
}

interface TablaGenericaProps<T> {
  columnas: ColumnaTabla<T>[];
  datos: T[];
  claveFila: (item: T) => string;
  acciones?: (item: T) => ReactNode;
  cargando?: boolean;
  mensajeVacio?: string;
}

export default function TablaGenerica<T>({
  columnas,
  datos,
  claveFila,
  acciones,
  cargando = false,
  mensajeVacio = 'No se encontraron registros.',
}: TablaGenericaProps<T>) {
  if (cargando) {
    return <div className="tabla-mensaje">Cargando...</div>;
  }

  if (datos.length === 0) {
    return <div className="tabla-mensaje">{mensajeVacio}</div>;
  }

  return (
    <div className="tabla-contenedor">
      <table className="tabla">
        <thead>
          <tr>
            {columnas.map((col, i) => (
              <th key={i} style={col.ancho ? { width: col.ancho } : undefined}>
                {col.encabezado}
              </th>
            ))}
            {acciones && <th style={{ width: '150px' }}>Acciones</th>}
          </tr>
        </thead>
        <tbody>
          {datos.map((item) => (
            <tr key={claveFila(item)}>
              {columnas.map((col, i) => (
                <td key={i}>
                  {typeof col.accesor === 'function'
                    ? col.accesor(item)
                    : String(item[col.accesor] ?? '')}
                </td>
              ))}
              {acciones && <td className="tabla-acciones">{acciones(item)}</td>}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
