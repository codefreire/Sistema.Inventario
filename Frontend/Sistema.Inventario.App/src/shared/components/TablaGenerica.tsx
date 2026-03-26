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

/**
 * Tabla reutilizable tipada con genericos para renderizar colecciones heterogeneas.
 *
 * Uso de generico `T`:
 * - columnas puede leer propiedades de `T` con `keyof T`.
 * - accesor tambien admite una funcion para renderizado personalizado por fila.
 * - claveFila recibe `T` para generar una clave unica por registro.
 *
 * @template T Tipo de dato de cada fila.
 * @param {TablaGenericaProps<T>} props Propiedades de configuracion de la tabla.
 * @param {ColumnaTabla<T>[]} props.columnas Definicion de columnas (encabezado, accesor y ancho opcional).
 * @param {T[]} props.datos Datos a renderizar en el cuerpo de la tabla.
 * @param {(item: T) => string} props.claveFila Funcion para obtener una clave unica por fila.
 * @param {(item: T) => ReactNode} [props.acciones] Renderizador opcional de acciones por fila.
 * @param {boolean} [props.cargando=false] Indica si debe mostrarse el estado de carga.
 * @param {string} [props.mensajeVacio='No se encontraron registros.'] Mensaje a mostrar cuando no hay datos.
 * @returns {JSX.Element} Tabla renderizada o mensajes de estado (cargando/vacio).
 *
 * @example
 * <TablaGenerica
 *   columnas={[
 *     { encabezado: 'Nombre', accesor: 'nombre' },
 *     { encabezado: 'Precio', accesor: (p) => `$${p.precio.toFixed(2)}` },
 *   ]}
 *   datos={productos}
 *   claveFila={(p) => p.id}
 * />
 */
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
