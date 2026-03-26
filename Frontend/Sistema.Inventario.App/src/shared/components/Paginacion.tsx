interface PaginacionProps {
  paginaActual: number;
  totalPaginas: number;
  onCambiarPagina: (pagina: number) => void;
}

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
