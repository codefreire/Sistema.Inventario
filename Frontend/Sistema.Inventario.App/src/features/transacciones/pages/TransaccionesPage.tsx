import { useState, useCallback, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useTransacciones } from '../hooks/useTransacciones';
import { transaccionService } from '../services/transaccionService';
import { productoService } from '../../productos/services/productoService';
import type { Transaccion } from '../types/Transaccion';
import type { Producto } from '../../productos/types/Producto';
import { formatearMoneda } from '../../../shared/utils/formatCurrency';
import TablaGenerica from '../../../shared/components/TablaGenerica';
import type { ColumnaTabla } from '../../../shared/components/TablaGenerica';
import Paginacion from '../../../shared/components/Paginacion';
import Button from '../../../shared/components/Button';
import Notificacion from '../../../shared/components/Notificacion';
import ModalConfirmacion from '../../../shared/components/ModalConfirmacion';
import FiltrosTransacciones from '../components/FiltrosTransacciones';

export default function TransaccionesPage() {
  const navigate = useNavigate();
  const {
    transacciones,
    totalTransacciones,
    cargando,
    error,
    paginaActual,
    totalPaginas,
    filtros,
    setPaginaActual,
    setFiltros,
    recargar,
  } = useTransacciones();

  const [productos, setProductos] = useState<Producto[]>([]);
  const [notificacion, setNotificacion] = useState({ mensaje: '', tipo: 'exito' as 'exito' | 'error', visible: false });
  const [modalEliminar, setModalEliminar] = useState<{ visible: boolean; transaccion: Transaccion | null }>({
    visible: false,
    transaccion: null,
  });
  const [mostrarFiltros, setMostrarFiltros] = useState(false);

  useEffect(() => {
    productoService.obtenerTodos().then(setProductos).catch(() => {});
  }, []);

  const obtenerNombreProducto = (productoId: string) => {
    const producto = productos.find((p) => p.id === productoId);
    return producto ? producto.nombre : productoId.substring(0, 8) + '...';
  };

  const mostrarNotificacion = useCallback((mensaje: string, tipo: 'exito' | 'error') => {
    setNotificacion({ mensaje, tipo, visible: true });
  }, []);

  const handleEliminar = async () => {
    if (!modalEliminar.transaccion) return;
    try {
      await transaccionService.eliminar(modalEliminar.transaccion.id);
      mostrarNotificacion('Transacción eliminada correctamente.', 'exito');
      recargar();
    } catch (error) {
      const mensaje = error instanceof Error ? error.message : 'Error al eliminar la transacción.';
      mostrarNotificacion(mensaje, 'error');
    } finally {
      setModalEliminar({ visible: false, transaccion: null });
    }
  };

  const columnas: ColumnaTabla<Transaccion>[] = [
    { encabezado: 'Fecha', accesor: (t) => new Date(t.fecha).toLocaleDateString('es-EC') },
    { encabezado: 'Tipo', accesor: (t) => (
      <span className={`badge badge-${t.tipoTransaccion.toLowerCase() === 'compra' ? 'exito' : 'peligro'}`}>
        {t.tipoTransaccion}
      </span>
    )},
    { encabezado: 'Producto', accesor: (t) => obtenerNombreProducto(t.productoId) },
    { encabezado: 'Cantidad', accesor: 'cantidad' },
    { encabezado: 'P. Unitario', accesor: (t) => formatearMoneda(t.precioUnitario) },
    { encabezado: 'Total', accesor: (t) => formatearMoneda(t.precioTotal) },
  ];

  return (
    <div className="pagina">
      <Notificacion
        mensaje={notificacion.mensaje}
        tipo={notificacion.tipo}
        visible={notificacion.visible}
        onCerrar={() => setNotificacion((n) => ({ ...n, visible: false }))}
      />

      <div className="pagina-encabezado">
        <h1>Transacciones</h1>
        <div className="pagina-acciones">
          <Button variante="secundario" onClick={() => setMostrarFiltros(!mostrarFiltros)}>
            {mostrarFiltros ? 'Ocultar Filtros' : '🔍 Filtros'}
          </Button>
          <Button onClick={() => navigate('/transacciones/crear')}>+ Nueva Transacción</Button>
        </div>
      </div>

      {mostrarFiltros && <FiltrosTransacciones filtros={filtros} onCambiarFiltros={setFiltros} productos={productos} />}

      {error && <div className="mensaje-error">{error}</div>}

      <p className="total-registros">{totalTransacciones} transacción(es) encontrada(s)</p>

      <TablaGenerica
        columnas={columnas}
        datos={transacciones}
        claveFila={(t) => t.id}
        cargando={cargando}
        mensajeVacio="No se encontraron transacciones."
        acciones={(transaccion) => (
          <>
            <Button variante="secundario" onClick={() => navigate(`/transacciones/editar/${transaccion.id}`)}>
              ✏️ Editar
            </Button>
            <Button variante="peligro" onClick={() => setModalEliminar({ visible: true, transaccion })}>
              🗑️ Eliminar
            </Button>
          </>
        )}
      />

      <Paginacion paginaActual={paginaActual} totalPaginas={totalPaginas} onCambiarPagina={setPaginaActual} />

      <ModalConfirmacion
        visible={modalEliminar.visible}
        titulo="Eliminar Transacción"
        mensaje="¿Está seguro de eliminar esta transacción?"
        onConfirmar={handleEliminar}
        onCancelar={() => setModalEliminar({ visible: false, transaccion: null })}
      />
    </div>
  );
}