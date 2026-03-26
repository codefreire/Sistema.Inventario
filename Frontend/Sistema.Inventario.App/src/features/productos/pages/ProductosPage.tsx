import { useState, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { useProductos } from '../hooks/useProductos';
import { productoService } from '../services/productoService';
import type { Producto } from '../types/Producto';
import { formatearMoneda } from '../../../shared/utils/formatCurrency';
import TablaGenerica from '../../../shared/components/TablaGenerica';
import type { ColumnaTabla } from '../../../shared/components/TablaGenerica';
import Paginacion from '../../../shared/components/Paginacion';
import Button from '../../../shared/components/Button';
import Notificacion from '../../../shared/components/Notificacion';
import ModalConfirmacion from '../../../shared/components/ModalConfirmacion';
import FiltrosProductos from '../components/FiltrosProductos';

export default function ProductosPage() {
  const navigate = useNavigate();
  const {
    productos,
    totalProductos,
    cargando,
    error,
    paginaActual,
    totalPaginas,
    filtros,
    setPaginaActual,
    setFiltros,
    recargar,
  } = useProductos();

  const [notificacion, setNotificacion] = useState({ mensaje: '', tipo: 'exito' as 'exito' | 'error', visible: false });
  const [modalEliminar, setModalEliminar] = useState<{ visible: boolean; producto: Producto | null }>({
    visible: false,
    producto: null,
  });
  const [mostrarFiltros, setMostrarFiltros] = useState(false);

  const mostrarNotificacion = useCallback((mensaje: string, tipo: 'exito' | 'error') => {
    setNotificacion({ mensaje, tipo, visible: true });
  }, []);

  const handleEliminar = async () => {
    if (!modalEliminar.producto) return;
    try {
      await productoService.eliminar(modalEliminar.producto.id);
      mostrarNotificacion('Producto eliminado correctamente.', 'exito');
      recargar();
    } catch {
      mostrarNotificacion('Error al eliminar el producto.', 'error');
    } finally {
      setModalEliminar({ visible: false, producto: null });
    }
  };

  const columnas: ColumnaTabla<Producto>[] = [
    { encabezado: 'Nombre', accesor: 'nombre' },
    { encabezado: 'Categoría', accesor: 'categoria' },
    { encabezado: 'Precio', accesor: (p) => formatearMoneda(p.precio) },
    { encabezado: 'Stock', accesor: (p) => <span className={p.stock === 0 ? 'texto-peligro' : ''}>{p.stock}</span> },
    { encabezado: 'Descripción', accesor: (p) => p.descripcion.length > 40 ? `${p.descripcion.substring(0, 40)}...` : p.descripcion },
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
        <h1>Productos</h1>
        <div className="pagina-acciones">
          <Button variante="secundario" onClick={() => setMostrarFiltros(!mostrarFiltros)}>
            {mostrarFiltros ? 'Ocultar Filtros' : '🔍 Filtros'}
          </Button>
          <Button onClick={() => navigate('/productos/crear')}>+ Nuevo Producto</Button>
        </div>
      </div>

      {mostrarFiltros && <FiltrosProductos filtros={filtros} onCambiarFiltros={setFiltros} />}

      {error && <div className="mensaje-error">{error}</div>}

      <p className="total-registros">{totalProductos} producto(s) encontrado(s)</p>

      <TablaGenerica
        columnas={columnas}
        datos={productos}
        claveFila={(p) => p.id}
        cargando={cargando}
        mensajeVacio="No se encontraron productos."
        acciones={(producto) => (
          <>
            <Button variante="secundario" onClick={() => navigate(`/productos/editar/${producto.id}`)}>
              ✏️ Editar
            </Button>
            <Button variante="peligro" onClick={() => setModalEliminar({ visible: true, producto })}>
              🗑️ Eliminar
            </Button>
          </>
        )}
      />

      <Paginacion paginaActual={paginaActual} totalPaginas={totalPaginas} onCambiarPagina={setPaginaActual} />

      <ModalConfirmacion
        visible={modalEliminar.visible}
        titulo="Eliminar Producto"
        mensaje={`¿Está seguro de eliminar el producto "${modalEliminar.producto?.nombre}"?`}
        onConfirmar={handleEliminar}
        onCancelar={() => setModalEliminar({ visible: false, producto: null })}
      />
    </div>
  );
}