import { useState, useEffect, useCallback } from 'react';
import { productoService } from '../services/productoService';
import type { Producto, FiltrosProducto } from '../types/Producto';

const ELEMENTOS_POR_PAGINA = 5;

/**
 * Hook para gestionar el listado de productos con carga remota, filtros y paginacion.
 *
 * Estado gestionado internamente:
 * - productos: lista base obtenida desde el servicio.
 * - productosFiltrados: lista resultante tras aplicar filtros.
 * - cargando y error: estado de ciclo de vida de la carga.
 * - paginaActual: pagina activa para el recorte de resultados.
 * - filtros: criterios de busqueda por nombre, categoria, precio y stock.
 *
 * @param {void} _ No recibe parametros.
 * @returns {{
 *   productos: Producto[];
 *   totalProductos: number;
 *   cargando: boolean;
 *   error: string | null;
 *   paginaActual: number;
 *   totalPaginas: number;
 *   filtros: FiltrosProducto;
 *   setPaginaActual: import('react').Dispatch<import('react').SetStateAction<number>>;
 *   setFiltros: import('react').Dispatch<import('react').SetStateAction<FiltrosProducto>>;
 *   recargar: () => Promise<void>;
 * }} Estado y acciones para renderizar, filtrar, paginar y recargar productos.
 *
 * @example
 * const { productos, filtros, setFiltros, paginaActual, setPaginaActual } = useProductos();
 * setFiltros((prev) => ({ ...prev, categoria: 'Electronica' }));
 * setPaginaActual(2);
 */
export function useProductos() {
  const [productos, setProductos] = useState<Producto[]>([]);
  const [productosFiltrados, setProductosFiltrados] = useState<Producto[]>([]);
  const [cargando, setCargando] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [paginaActual, setPaginaActual] = useState(1);
  const [filtros, setFiltros] = useState<FiltrosProducto>({});

  const cargarProductos = useCallback(async () => {
    try {
      setCargando(true);
      setError(null);
      const datos = await productoService.obtenerTodos();
      setProductos(datos);
      setProductosFiltrados(datos);
    } catch {
      setError('Error al cargar los productos.');
    } finally {
      setCargando(false);
    }
  }, []);

  useEffect(() => {
    cargarProductos();
  }, [cargarProductos]);

  useEffect(() => {
    let resultado = [...productos];

    if (filtros.nombre) {
      resultado = resultado.filter((p) =>
        p.nombre.toLowerCase().includes(filtros.nombre!.toLowerCase())
      );
    }
    if (filtros.categoria) {
      resultado = resultado.filter((p) =>
        p.categoria.toLowerCase().includes(filtros.categoria!.toLowerCase())
      );
    }
    if (filtros.precioMin !== undefined && filtros.precioMin !== null) {
      resultado = resultado.filter((p) => p.precio >= filtros.precioMin!);
    }
    if (filtros.precioMax !== undefined && filtros.precioMax !== null) {
      resultado = resultado.filter((p) => p.precio <= filtros.precioMax!);
    }
    if (filtros.stockMin !== undefined && filtros.stockMin !== null) {
      resultado = resultado.filter((p) => p.stock >= filtros.stockMin!);
    }
    if (filtros.stockMax !== undefined && filtros.stockMax !== null) {
      resultado = resultado.filter((p) => p.stock <= filtros.stockMax!);
    }

    setProductosFiltrados(resultado);
    setPaginaActual(1);
  }, [filtros, productos]);

  const totalPaginas = Math.ceil(productosFiltrados.length / ELEMENTOS_POR_PAGINA);
  const productosPaginados = productosFiltrados.slice(
    (paginaActual - 1) * ELEMENTOS_POR_PAGINA,
    paginaActual * ELEMENTOS_POR_PAGINA
  );

  return {
    productos: productosPaginados,
    totalProductos: productosFiltrados.length,
    cargando,
    error,
    paginaActual,
    totalPaginas,
    filtros,
    setPaginaActual,
    setFiltros,
    recargar: cargarProductos,
  };
}