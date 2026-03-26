import { useState, useEffect, useCallback } from 'react';
import { transaccionService } from '../services/transaccionService';
import type { Transaccion, FiltrosTransaccion } from '../types/Transaccion';

const ELEMENTOS_POR_PAGINA = 5;

export function useTransacciones() {
  const [transacciones, setTransacciones] = useState<Transaccion[]>([]);
  const [transaccionesFiltradas, setTransaccionesFiltradas] = useState<Transaccion[]>([]);
  const [cargando, setCargando] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [paginaActual, setPaginaActual] = useState(1);
  const [filtros, setFiltros] = useState<FiltrosTransaccion>({});

  const cargarTransacciones = useCallback(async () => {
    try {
      setCargando(true);
      setError(null);
      const datos = await transaccionService.obtenerTodas();
      setTransacciones(datos);
      setTransaccionesFiltradas(datos);
    } catch {
      setError('Error al cargar las transacciones.');
    } finally {
      setCargando(false);
    }
  }, []);

  useEffect(() => {
    cargarTransacciones();
  }, [cargarTransacciones]);

  useEffect(() => {
    let resultado = [...transacciones];

    if (filtros.tipoTransaccion) {
      resultado = resultado.filter((t) =>
        t.tipoTransaccion.toLowerCase() === filtros.tipoTransaccion!.toLowerCase()
      );
    }
    if (filtros.productoId) {
      resultado = resultado.filter((t) => t.productoId === filtros.productoId);
    }
    if (filtros.fechaDesde) {
      resultado = resultado.filter((t) => new Date(t.fecha) >= new Date(filtros.fechaDesde!));
    }
    if (filtros.fechaHasta) {
      resultado = resultado.filter((t) => new Date(t.fecha) <= new Date(filtros.fechaHasta!));
    }
    if (filtros.cantidadMin !== undefined && filtros.cantidadMin !== null) {
      resultado = resultado.filter((t) => t.cantidad >= filtros.cantidadMin!);
    }
    if (filtros.cantidadMax !== undefined && filtros.cantidadMax !== null) {
      resultado = resultado.filter((t) => t.cantidad <= filtros.cantidadMax!);
    }

    setTransaccionesFiltradas(resultado);
    setPaginaActual(1);
  }, [filtros, transacciones]);

  const totalPaginas = Math.ceil(transaccionesFiltradas.length / ELEMENTOS_POR_PAGINA);
  const transaccionesPaginadas = transaccionesFiltradas.slice(
    (paginaActual - 1) * ELEMENTOS_POR_PAGINA,
    paginaActual * ELEMENTOS_POR_PAGINA
  );

  return {
    transacciones: transaccionesPaginadas,
    totalTransacciones: transaccionesFiltradas.length,
    cargando,
    error,
    paginaActual,
    totalPaginas,
    filtros,
    setPaginaActual,
    setFiltros,
    recargar: cargarTransacciones,
  };
}