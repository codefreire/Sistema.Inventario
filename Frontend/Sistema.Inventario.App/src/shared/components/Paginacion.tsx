interface PaginacionProps {
  paginaActual: number;
  totalPaginas: number;
  onCambiarPagina: (pagina: number) => void;
}

/**
 * Componente de paginacion reutilizable para listados tabulares.
 *
 * Comportamiento:
 * - Si `totalPaginas` es menor o igual a 1, no renderiza controles.
 * - Muestra un rango centrado en la pagina actual (radio de 2 paginas por lado).
 * - Incluye botones para primera/ultima pagina cuando quedan fuera del rango visible.
 * - Deshabilita navegacion anterior/siguiente en los extremos.
 *
 * @param {PaginacionProps} props Propiedades de paginacion.
 * @param {number} props.paginaActual Pagina seleccionada actualmente (base 1).
 * @param {number} props.totalPaginas Total de paginas disponibles.
 * @param {(pagina: number) => void} props.onCambiarPagina Callback ejecutado al seleccionar una nueva pagina.
 * @returns {JSX.Element | null} Controles de paginacion o `null` cuando no aplica paginar.
 *
 * @example
 * <Paginacion
 *   paginaActual={paginaActual}
 *   totalPaginas={totalPaginas}
 *   onCambiarPagina={setPaginaActual}
 * />
 */
export default function Paginacion({ paginaActual, totalPaginas, onCambiarPagina }: PaginacionProps) {
  if (totalPaginas <= 1) return null;

  const paginas: number[] = [];
  const rango = 2;
  const inicio = Math.max(1, paginaActual - rango);
  const fin = Math.min(totalPaginas, paginaActual + rango);

  for (let i = inicio; i <= fin; i++) {
    paginas.push(i);
  }

  return (
    <div className="paginacion">
      <button
        className="btn btn-secundario btn-sm"
        disabled={paginaActual === 1}
        onClick={() => onCambiarPagina(paginaActual - 1)}
      >
        « Anterior
      </button>

      {inicio > 1 && (
        <>
          <button className="btn btn-secundario btn-sm" onClick={() => onCambiarPagina(1)}>1</button>
          {inicio > 2 && <span className="paginacion-ellipsis">...</span>}
        </>
      )}

      {paginas.map((pagina) => (
        <button
          key={pagina}
          className={`btn btn-sm ${pagina === paginaActual ? 'btn-primario' : 'btn-secundario'}`}
          onClick={() => onCambiarPagina(pagina)}
        >
          {pagina}
        </button>
      ))}

      {fin < totalPaginas && (
        <>
          {fin < totalPaginas - 1 && <span className="paginacion-ellipsis">...</span>}
          <button className="btn btn-secundario btn-sm" onClick={() => onCambiarPagina(totalPaginas)}>
            {totalPaginas}
          </button>
        </>
      )}

      <button
        className="btn btn-secundario btn-sm"
        disabled={paginaActual === totalPaginas}
        onClick={() => onCambiarPagina(paginaActual + 1)}
      >
        Siguiente »
      </button>
    </div>
  );
}
